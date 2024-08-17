using Mio.Reader.Parsing.Structure;
using Mio.Reader.Parsing.Structure.Chars;
using Mio.Reader.Services;
using Mio.Reader.Utilitarians;
using Mio.Translation;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mio.Reader.Parsing
{

    internal abstract class Parser
    {
        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        protected static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        protected static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";

        //Obviouslt breaks if configs is not assigned before analyzer, but that should never happen because this field is assinged in the very ConfigurationsService constructor.
        protected ConfigurationsService configs;
        public static Analyzer? analyzer;

        protected Translator translator = new Translator();

        protected ImageParsingService imageParser;

        public Parser(ConfigurationsService configs, ImageParsingService imageParsingService)
        {
            this.configs = configs;
            imageParser = imageParsingService;
        }
        public abstract Task<List<Node>> ParseLine(Chapter chapter, int lineIndex);


        protected abstract  Task<List<Node>> ParseTextElement(ParsingElement originalElement);

        protected abstract Task<List<Node>> ParseImageElement(Chapter chapter, ParsingElement originalElement, string srcAttribute);

        public abstract Task<int> BreakChapterToLines(Chapter chapter);
    }
}
