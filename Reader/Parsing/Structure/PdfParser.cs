using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    internal class PdfParser : Parser
    {
        public PdfParser(ConfigurationsService configs, ImageParsingService imageParsingService) : base(configs, imageParsingService)
        {
        }

        public override Task<int> BreakChapterToLines(Chapter chapter)
        {
            throw new NotImplementedException();
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
    }
}
