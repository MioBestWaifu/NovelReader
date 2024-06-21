using MeCab;
using Mio.Translation;

namespace Mio.Translation.Japanese
{
    public class JapaneseAnalyzer
    {
        readonly MeCabTagger tagger;

        public static readonly GrammaticalCategory[] signicantCategories = { GrammaticalCategory.Noun, GrammaticalCategory.Verb, GrammaticalCategory.Adjective, GrammaticalCategory.Adverb, GrammaticalCategory.Pronoun };

        public JapaneseAnalyzer(string pathToUnidic)
        {
            //<MeCabUseDefaultDictionary>False</MeCabUseDefaultDictionary>
            var parameter = new MeCabParam();
            parameter.DicDir = pathToUnidic;
            tagger = MeCabTagger.Create(parameter);
        }

        /// <summary>
        /// Also includes errored nodes in the resulting list, with GramaticalCategory of Error. This is to ensure the
        /// integrity of the text. Data is not to be lost here if the analyzer fails to parse a node.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Always a initialized list</returns>
        public List<JapaneseLexeme> Analyze(string text)
        {
            var nodes = tagger.ParseToNodes(text);
            var lexemes = new List<JapaneseLexeme>();
            foreach (var node in nodes)
            {
                if (node.Feature is not null)
                {
                    var features = node.Feature.Split(',');
                    try
                    {
                        //Some interesting thins are being ignored here. Like, it says the conjugation form and type.
                        var lexeme = new JapaneseLexeme(node.Surface, features);
                        lexemes.Add(lexeme);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error parsing node: {node.Surface} {node.Feature}");
                        Console.WriteLine(e.Message);
                        lexemes.Add(new JapaneseLexeme(node.Surface));
                    }
                }
            }
            return lexemes;
        }

        public static GrammaticalCategory ParseToCategory(string category)
        {
            return category switch
            {
                "名詞" => GrammaticalCategory.Noun,
                "動詞" => GrammaticalCategory.Verb,
                "形容詞" => GrammaticalCategory.Adjective,
                "副詞" => GrammaticalCategory.Adverb,
                "助詞" => GrammaticalCategory.Particle,
                "助動詞" => GrammaticalCategory.AuxiliaryVerb,
                "接続詞" => GrammaticalCategory.Conjunction,
                "感動詞" => GrammaticalCategory.Interjection,
                "記号" => GrammaticalCategory.Symbol,
                "フィラー" => GrammaticalCategory.Filler,
                "代名詞" => GrammaticalCategory.Pronoun,
                "その他" => GrammaticalCategory.Other,
                _ => GrammaticalCategory.Unknown,
            };
        }

        public static int DetermineKanjiNumber(char kanji)
        {
            int unicodeValue = kanji;

            // Define the start and end of each Unicode range for kanji
            int range1Start = 0x4E00, range1End = 0x9FFF;
            int range2Start = 0x3400, range2End = 0x4DBF;
            int range3Start = 0x20000, range3End = 0x2A6DF;
            int range4Start = 0x2A700, range4End = 0x2EBEF;

            // Calculate the offsets for the starting indices of each range
            int range1Size = range1End - range1Start + 1;
            int range2Size = range2End - range2Start + 1;
            int range3Size = range3End - range3Start + 1;

            if (unicodeValue >= range1Start && unicodeValue <= range1End)
            {
                return unicodeValue - range1Start;
            }
            else if (unicodeValue >= range2Start && unicodeValue <= range2End)
            {
                return range1Size + (unicodeValue - range2Start);
            }
            else if (unicodeValue >= range3Start && unicodeValue <= range3End)
            {
                return range1Size + range2Size + (unicodeValue - range3Start);
            }
            else if (unicodeValue >= range4Start && unicodeValue <= range4End)
            {
                return range1Size + range2Size + range3Size + (unicodeValue - range4Start);
            }
            else
            {
                throw new ArgumentException("The provided character is not a kanji within the specified ranges.");
            }
        }

        public static bool IsKana(char c)
        {
            return c >= 0x3040 && c <= 0x309F || c >= 0x30A0 && c <= 0x30FF;
        }

        public static bool IsRomaji(char c)
        {
            return c >= 0x0041 && c <= 0x005A || c >= 0x0061 && c <= 0x007A;
        }
    }
}
