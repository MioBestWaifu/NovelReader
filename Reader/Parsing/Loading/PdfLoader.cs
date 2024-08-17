using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Loading
{
    internal class PdfLoader : BookLoader
    {
        public PdfLoader(ConfigurationsService configs, ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
            parser = new PdfParser(configs, imageParsingService);
        }

        public override Task<Book> IndexBook(BookMetadata metadata)
        {
            throw new NotImplementedException();
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
                        metadata.CoverBase64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension(),newWidth,newHeight);
                        break;
                    }
                }
                return metadata.CoverBase64 != "";
            }
        }

        public override async Task<bool> LoadCover(BookMetadata metadata)
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
                        metadata.CoverBase64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension());
                        break;
                    }
                }
                return metadata.CoverBase64 != "";
            }
        }

        public override async Task<BookMetadata> LoadMetadata(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                // Extract metadata
                PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
                string title = info.GetTitle();
                string author = info.GetAuthor();

                // Extract cover image (assuming it's the first image on the first page)
                PdfPage firstPage = pdfDocument.GetFirstPage();
                var resources = firstPage.GetResources();
                var xObjects = resources.GetResourceNames();
                string cover64 = "";
                foreach (var name in xObjects)
                {
                    PdfImageXObject obj = resources.GetImage(name);

                    if (obj != null )
                    {
                        var imageBytes = obj.GetImageBytes();
                        cover64 = await imageParser.ParseImageBytesToBase64(imageBytes, obj.IdentifyImageFileExtension(), 440, 660);
                        break;
                    }
                }
                return new BookMetadata(title, author, path, -1,cover64,"","");
            }

        }

        public override Task ParseChapterContent(Chapter chapter, IProgress<int> progressReporter)
        {
            throw new NotImplementedException();
        }
    }
}
