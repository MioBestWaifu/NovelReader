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

    public abstract class Parser
    {
        //([,!?、。．。「」『』…．！？：；（）()'\"“”])";
        //include any other separators that might be missing
        //Why two fields? Class regex as above is fast for regexing, list is faster for comparing.
        protected static readonly List<string> separatorsAsList = new List<string> { ",", "!", "?", "、", "。", "．", "「", "」", "『", "』", "…", "．", "！", "？", "：", "；", "（", "）", "(", ")", "'", "\"", "“", "”" };

        protected static readonly string separatorsRegex = "([" + string.Join("", separatorsAsList.Select(Regex.Escape)) + "])";

        //Obviouslt breaks if configs is not assigned before analyzer, but that should never happen because this field is assinged in the very ConfigurationsService constructor.
        protected ConfigurationsService configs;
        protected static Analyzer? analyzer;

        protected Translator translator = new Translator();

        protected ImageParsingService imageParser;

        public Parser(ConfigurationsService configs, ImageParsingService imageParsingService)
        {
            this.configs = configs;
            imageParser = imageParsingService;
            if (analyzer == null)
            {
                analyzer = new Analyzer(configs.PathToUnidic);
            }
        }
        public abstract Task<List<Node>> ParseLine(Chapter chapter, int lineIndex);


        protected abstract  Task<List<Node>> ParseTextElement(ParsingElement originalElement);

        protected async Task<List<Node>> ParseTextElement(string text)
        {
            if (text == string.Empty)
            {
                return [new TextNode()];
            }
            //Breaking lines into sentences for smoother morphological analysis
            string[] sentences = Regex.Split(text, separatorsRegex);
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

            return nodes;
        }

        protected abstract Task<List<Node>> ParseImageElement(Chapter chapter, ParsingElement originalElement, string srcAttribute);

        public abstract Task<int> BreakChapterToLines(Chapter chapter);
    }
}
