using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;

using ITextPdfDocument = iText.Kernel.Pdf.PdfDocument;
using PigPdfDocument = UglyToad.PdfPig.PdfDocument;
using PigPage = UglyToad.PdfPig.Content.Page;
using System.Collections.Concurrent;

namespace Mio.Reader.Parsing.Loading
{
    internal class PdfLoader : BookLoader
    {
        private Pdf pdf;
        public PdfLoader(ConfigurationsService configs, ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
            parser = new PdfParser(configs, imageParsingService);
        }

        public override async Task<bool> LoadAndResizeCover(BookMetadata metadata, int newWidth, int newHeight)
        {

            using (PdfReader reader = new PdfReader(metadata.Path))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                // Extract cover image (assuming it's the first image on the first page)
                PdfPage firstPage = pdfDocument.GetFirstPage();
                var resources = firstPage.GetResources();
                var xObjects = resources.GetResourceNames();
                string cover64 = "";
                foreach (var name in xObjects)
                {
                    PdfImageXObject obj = resources.GetImage(name);

                    if (obj != null)
                    {
                        var imageBytes = obj.GetImageBytes();
                        metadata.CoverBase64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension(), newWidth, newHeight);
                        break;
                    }
                }
                return metadata.CoverBase64 != "";
            }
        }

        public override async Task<bool> LoadCover(BookMetadata metadata)
        {
            using (PdfReader reader = new PdfReader(metadata.Path))
            using (ITextPdfDocument iTextPdf = new ITextPdfDocument(reader))
            {
                // Extract cover image (assuming it's the first image on the first page)
                PdfPage firstPage = iTextPdf.GetFirstPage();
                var resources = firstPage.GetResources();
                var xObjects = resources.GetResourceNames();
                string cover64 = "";
                foreach (var name in xObjects)
                {
                    PdfImageXObject obj = resources.GetImage(name);

                    if (obj != null)
                    {
                        var imageBytes = obj.GetImageBytes();
                        metadata.CoverBase64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension());
                        break;
                    }
                }
                return metadata.CoverBase64 != "";
            }
        }

        public override async Task<BookMetadata> LoadMetadata(string path)
        {
            int numberOfPages;
            string title, author, cover64;
            using (PdfReader reader = new PdfReader(path))
            using (ITextPdfDocument iTextPdf = new ITextPdfDocument(reader))
            {
                // Extract metadata
                PdfDocumentInfo info = iTextPdf.GetDocumentInfo();
                title = info.GetTitle();
                author = info.GetAuthor();
                numberOfPages = iTextPdf.GetNumberOfPages();

                // Extract cover image (assuming it's the first image on the first page)
                PdfPage firstPage = iTextPdf.GetFirstPage();
                var resources = firstPage.GetResources();
                var xObjects = resources.GetResourceNames();
                cover64 = "";
                foreach (var name in xObjects)
                {
                    PdfImageXObject obj = resources.GetImage(name);

                    if (obj != null)
                    {
                        var imageBytes = obj.GetImageBytes();
                        cover64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension(), 440, 660);
                        break;
                    }
                }
            }
            
            List<int> pagesToSearch;
            if (numberOfPages > 10)
            {
                pagesToSearch = [];
                Random random = new Random();
                for (int i = 1; i <= 10; i++)
                {
                    pagesToSearch.Add(random.Next(1, numberOfPages));
                }
            }
            else
            {
                pagesToSearch = Enumerable.Range(1, numberOfPages).ToList();
            }

            double maxLeft = 0, maxBot = 0;
            ConcurrentDictionary<double, int> fontSizes = [];

            using (PigPdfDocument pigPdf = PigPdfDocument.Open(path))
            {
                
                foreach (int pageIndex in pagesToSearch)
                {
                    PigPage page = pigPdf.GetPage(pageIndex);
                    Parallel.ForEach(page.GetWords(), (word) =>
                    {
                        var rightEdge = word.BoundingBox.BottomRight.X;
                        var bottomEdge = word.BoundingBox.BottomRight.Y;
                        if (rightEdge > maxLeft)
                        {
                            maxLeft = rightEdge;
                        }
                        if (bottomEdge > maxBot)
                        {
                            maxBot = bottomEdge;
                        }
                        fontSizes.AddOrUpdate(word.Letters[0].FontSize, 1, (key, oldValue) => oldValue + 1);
                    });
                }
            };

            PdfMetadata toReturn = new PdfMetadata(title, author, path, cover64);
            toReturn.MarginBottom = maxBot;
            toReturn.MarginRight = maxLeft;
            toReturn.PageCount = numberOfPages;
            toReturn.BodyFontSize = fontSizes.OrderByDescending(x => x.Value).First().Key;
            return toReturn;
        }

        public override Task ParseChapterContent(Chapter chapter, IProgress<int> progressReporter)
        {
            PreparePdfNodes(chapter as PdfChapter);
            return base.ParseChapterContent(chapter, progressReporter);
        }

        /// <summary>
        /// Removes things that are not to be parsed (eg furigana) and joins lines that are visually separated but are actually the same.
        /// </summary>
        /// <param name="chapter"></param>
        private void PreparePdfNodes(PdfChapter chapter)
        {
            PdfMetadata metadata = (PdfMetadata)pdf.Metadata;
            for (int i = 0; i < chapter.OriginalLines.Count; i++)
            {
                PdfNode pdfNode = new PdfNode();
                int thisIteration = i;
                PdfParsingElement currentElement = chapter.OriginalLines[i] as PdfParsingElement;
                if (currentElement.FontSize <= metadata.BodyFontSize * 0.8)
                {
                    pdfNode.IsFurigana = true;
                    pdfNode.FontSize = currentElement.FontSize;
                }
                //Determines if this lines is possibly part of the same phrase as part of the next line.
                if (currentElement.RightMostPoint > metadata.MarginRight * 0.98) {
                    if (i + 1 < chapter.OriginalLines.Count)
                    {
                        PdfParsingElement nextElement = chapter.OriginalLines[i + 1] as PdfParsingElement;
                        if (currentElement.Page == nextElement.Page && !nextElement.IsImage)
                        {
                            pdfNode.PossibleSharing = true;
                        }
                    } 
                }

                chapter.PdfLines.Add(pdfNode);
                chapter.AddLineMapping(currentElement.Page, i);
            }
        }

        public override async Task<Book> IndexBook(BookMetadata metadata)
        {
            //Shit may happen here
            PdfMetadata pdfMetadata = metadata as PdfMetadata;
            var book = new Pdf(pdfMetadata);
            var chapters = new List<PdfChapter>();
            using (PdfReader reader = new PdfReader(metadata.Path))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                // Attempt to load the bookmarks (usually represent TOC)
                PdfOutline outlines = pdfDocument.GetOutlines(false);
                if (outlines is not null && outlines.GetAllChildren().Count > 0)
                {
                    ProcessOutlines(outlines, chapters, pdfDocument);
                }
                else
                {
                    // Create artificial chapters if no TOC is available
                    CreateArtificialChapters(chapters, pdfDocument);
                }
            };

            book.TableOfContents = [];
            foreach (var chapter in chapters)
            {
                chapter.pathToPdf = metadata.Path;
                book.TableOfContents.Add((chapter.Title,chapter));
            }
            pdf = book;
            return book;
        }

        private void ProcessOutlines(PdfOutline outlines, List<PdfChapter> chapters, PdfDocument pdfDocument)
        {
            foreach (var outline in outlines.GetAllChildren())
            {
                ProcessOutlineItem(outline, chapters, pdfDocument);
            }

            // Ensure every page is part of a chapter
            AddRemainingPagesToChapters(chapters, pdfDocument.GetNumberOfPages());
        }

        private void ProcessOutlineItem(PdfOutline outline, List<PdfChapter> chapters, PdfDocument pdfDocument)
        {
            int startPage = GetPageFromDestination(outline.GetDestination(), pdfDocument);

            /*// Recursively process child outlines (subsections)
            if (outline.GetAllChildren().Any())
            {
                ProcessOutlines(outline.GetAllChildren(), chapters, pdfDocument);
            }*/

            //ends pages are -1 because they will be filled based on the start page of the next chapter
            chapters.Add(new PdfChapter(
                outline.GetTitle(),
                startPage,
                -1
            ));
        }

        private int GetPageFromDestination(PdfDestination destination, PdfDocument pdfDocument)
        {
            if (destination != null)
            {
                // Get the PdfObject that represents the page destination
                PdfObject destinationPage = destination.GetDestinationPage(pdfDocument.GetCatalog().GetNameTree(PdfName.Dests));
                if(destinationPage is null)
                {
                    return -1;
                } else if (destinationPage.IsIndirectReference())
                {
                    // Resolve the indirect reference to get the actual PdfPage
                    PdfPage page = pdfDocument.GetPage((PdfDictionary)pdfDocument.GetPdfObject(destinationPage.GetIndirectReference().GetObjNumber()));

                  if (page != null)
                  {
                      return pdfDocument.GetPageNumber(page);
                  }
                } 
                //Dont know how sensible this is
                else
                {
                    bool x = destinationPage.IsDictionary();
                    bool y = destinationPage.IsArray();
                    bool z = destinationPage.IsStream();
                    PdfPage page = pdfDocument.GetPage((PdfDictionary)destinationPage);

                    if (page != null)
                    {
                        return pdfDocument.GetPageNumber(page);
                    }
                }
            }

            // Default to the first page if the destination is not recognized or is null
            return 1;
        }


        private void CreateArtificialChapters(List<PdfChapter> chapters, PdfDocument pdfDocument)
        {
            int totalPages = pdfDocument.GetNumberOfPages();
            int pagesPerChapter = 15;
            for (int i = 1; i <= totalPages; i += pagesPerChapter)
            {
                chapters.Add(new PdfChapter(

                    $"Chapter {chapters.Count + 1}",
                    i,
                    Math.Min(i + pagesPerChapter - 1, totalPages)
                ));
            }
        }

        /// <summary>
        /// Ensures that every page is part o a chapter even if TOC says otherwise.
        /// </summary>
        /// <param name="chapters"></param>
        /// <param name="totalPages"></param>
        private void AddRemainingPagesToChapters(List<PdfChapter> chapters, int totalPages)
        {
            chapters[0].startPage = 1;
            chapters[^1].endPage = totalPages;

            if(chapters.Count > 1)
            {
                for (int i = 1; i < chapters.Count; i++)
                {
                    chapters[i - 1].endPage = chapters[i].startPage - 1;
                }
            }
        }

    }
}
