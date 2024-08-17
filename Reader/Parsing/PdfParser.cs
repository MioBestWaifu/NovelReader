using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (PdfReader reader = new PdfReader(chapter.pathToPdf))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                for (int pageNumber = chapter.startPage; pageNumber <= chapter.endPage; pageNumber++)
                {
                    PdfPage page = pdfDocument.GetPage(pageNumber);
                    PdfCanvasProcessor processor = new PdfCanvasProcessor(new RenderListener(lines));
                    processor.ProcessPageContent(page);
                }

                chapter.OriginalLines = [];
                foreach (PdfParsingElement element in lines)
                {
                    chapter.OriginalLines.Add(element);
                }

                return lines.Count;
            };
        }

        public override Task<List<Node>> ParseLine(Chapter chapter, int lineIndex)
        {
            throw new NotImplementedException();
        }

        protected override Task<List<Node>> ParseImageElement(Chapter chapter, ParsingElement originalElement, string srcAttribute)
        {
            throw new NotImplementedException();
        }

        protected override Task<List<Node>> ParseTextElement(ParsingElement originalElement)
        {
            throw new NotImplementedException();
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
                    case EventType.RENDER_TEXT:
                        TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                        _contentObjects.Add(new PdfParsingElement { Text = textRenderInfo.GetText() , IsImage = false});
                        break;

                    case EventType.RENDER_IMAGE:
                        ImageRenderInfo imageRenderInfo = (ImageRenderInfo)data;
                        PdfImageXObject image = imageRenderInfo.GetImage();
                        if (image != null)
                        {
                            _contentObjects.Add(new PdfParsingElement { Image = image.GetImageBytes(), Extension = image.IdentifyImageFileExtension(), IsImage = true });
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
