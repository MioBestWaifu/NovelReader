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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITextPdfDocument = iText.Kernel.Pdf.PdfDocument;
using PigPdfDocument = UglyToad.PdfPig.PdfDocument;
using PigPage = UglyToad.PdfPig.Content.Page;
using ITextPage = iText.Kernel.Pdf.PdfPage;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using iText.Kernel.Geom;
using Mio.Translation;
using System.Text.RegularExpressions;
using Mio.Reader.Parsing.Structure.Chars;
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
            string[] sentences = Regex.Split(element.Text, separatorsRegex);
            List<Node> nodes = new List<Node>();
            for (int i = 0, n = sentences.Length; i < n; i++)
            {
                //Should never be zero, i think. If it happens, will cause a bug. Purposefully not checking to see if breaks.
                //Also, This means that the separators are not interactable as part of a word. This is not a problem, because separators ARE NOT words.
                if (sentences[i].Length == 1 && separatorsAsList.Contains(sentences[i]))
                {
                    if (nodes.Count == 0)
                    {
                        nodes.Add(new TextNode() { Characters = { new Yakumono(sentences[i][0]) } });
                    }
                    else
                    {
                        //This will break if the previoues node was not a textnode, but that should never happen.
                        TextNode nodeToAppend = (TextNode)nodes[^1];
                        nodeToAppend.Characters.Add(new Yakumono(sentences[i][0]));
                    }
                    continue;
                }

                List<Lexeme> lexemes = analyzer.Analyze(sentences[i]);
                foreach (var lexeme in lexemes)
                {
                    TextNode node = new TextNode(lexeme);
                    List<JapaneseCharacter> chars = [];
                    foreach (var character in lexeme.Surface)
                    {
                        if (Analyzer.IsRomaji(character))
                        {
                            chars.Add(new Romaji(character));
                        }
                        else if (Analyzer.IsKana(character))
                        {
                            bool isYoon = Analyzer.IsYoon(character);
                            Kana kana = new Kana(character, isYoon);
                            chars.Add(kana);
                        }
                        else if (Analyzer.IsKanji(character))
                        {
                            Kanji kanji = new Kanji(character);
                            chars.Add(kanji);
                        }
                        else
                        {
                            //Presuming anything that is not a kana, kanji or romaji is a yakumono.
                            //May or may not be a good idea.
                            chars.Add(new Yakumono(character));
                        }
                    }
                    node.Characters = chars;
                    nodes.Add(node);
                }
            }

            return Task.FromResult(nodes);
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
