using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using UglyToad.PdfPig;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using System;

using ITextPdfDocument = iText.Kernel.Pdf.PdfDocument;
using PigPdfDocument = UglyToad.PdfPig.PdfDocument;
using PigPage = UglyToad.PdfPig.Content.Page;
using ITextPage = iText.Kernel.Pdf.PdfPage;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using iText.Kernel.Geom;

namespace Mio.Reader.Parsing
{
    internal class PdfParser : Parser
    {
        public PdfParser(ConfigurationsService configs, ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
        }

        public override Task<int> BreakChapterToLines(Chapter chapter)
        {
            if (chapter is PdfChapter pdfChapter)
            {
                return BreakChapterToLines(pdfChapter);
            }
            else
            {
                throw new ArgumentException("Chapter is not a PdfChapter");
            }
        }

        private async Task<int> BreakChapterToLines (PdfChapter chapter)
        {
            List<PdfParsingElement> lines = new List<PdfParsingElement>();
            using (PigPdfDocument pigPdf = PigPdfDocument.Open(chapter.pathToPdf))
            {
                for (int i = chapter.startPage; i<=chapter.endPage; i++)
                {
                    PigPage page = pigPdf.GetPage(i);
                    foreach (Word word in page.GetWords())
                    {
                        lines.Add(new PdfParsingElement { Text = word.Text, IsImage = false, BoundingBox = word.BoundingBox, Page = i, FontSize = word.Letters[0].FontSize });
                    }
                }
            };

            using(ITextPdfDocument iTextPdf = new ITextPdfDocument(new PdfReader(chapter.pathToPdf)))
            {
                for (int pageNumber = chapter.startPage; pageNumber <= chapter.endPage; pageNumber++)
                {
                    ITextPage page = iTextPdf.GetPage(pageNumber);
                    PdfCanvasProcessor processor = new PdfCanvasProcessor(new RenderListener(lines));
                    processor.ProcessPageContent(page);
                }
            }
            chapter.OriginalLines = lines.Select(x=> x as ParsingElement).ToList();
            return lines.Count;
        }

        public override Task<List<Node>> ParseLine(Chapter chapter, int lineIndex)
        {
            PdfParsingElement element = chapter.OriginalLines[lineIndex] as PdfParsingElement;
            if (element is null)
            {
                throw new ArgumentException("Element is not a PdfParsingElement");
            }

            if (element.IsImage)
            {
                return ParseImageElement(chapter, element, "");
            }
            else
            {
                return ParseTextElement(element);
            }
        }

        protected override Task<List<Node>> ParseImageElement(Chapter chapter, ParsingElement originalElement, string srcAttribute)
        {
            throw new NotImplementedException();
        }

        protected override Task<List<Node>> ParseTextElement(ParsingElement originalElement)
        {
            PdfParsingElement element = originalElement as PdfParsingElement;
            if (element is null)
            {
                throw new ArgumentException("Element is not a PdfParsingElement");
            }
            return ParseTextElement(element.Text);
        }

        private class RenderListener : IEventListener
        {
            private readonly List<PdfParsingElement> _contentObjects;

            public RenderListener(List<PdfParsingElement> contentObjects)
            {
                _contentObjects = contentObjects;
            }

            public void EventOccurred(IEventData data, EventType type)
            {
                switch (type)
                {
                    /*case EventType.RENDER_TEXT:
                        TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                        _contentObjects.Add(new PdfParsingElement { Text = textRenderInfo.GetText(), IsImage = false });
                        break;
*/
                    case EventType.RENDER_IMAGE:
                        ImageRenderInfo imageRenderInfo = (ImageRenderInfo)data;
                        PdfImageXObject image = imageRenderInfo.GetImage();
                        if (image != null)
                        {
                            var imageBytes = image.GetImageBytes();
                            var imageCtm = imageRenderInfo.GetImageCtm();
                            var imageCoordinates = new PdfRectangle(
                                imageCtm.Get(Matrix.I31),
                                imageCtm.Get(Matrix.I32),
                                imageCtm.Get(Matrix.I11),
                                imageCtm.Get(Matrix.I22)
                            );

                            _contentObjects.Add(new PdfParsingElement
                            {
                                Image = imageBytes,
                                Extension = image.IdentifyImageFileExtension(),
                                IsImage = true,
                                BoundingBox = imageCoordinates
                            });
                        }
                        break;
                }
            }

            public ICollection<EventType> GetSupportedEvents()
            {
                return new HashSet<EventType> { EventType.RENDER_TEXT, EventType.RENDER_IMAGE };
            }
        }
    }

}
