using MeCab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Translation.Japanese
{
    internal class JapaneseAnalyzer
    {
        MeCabTagger tagger;

        public JapaneseAnalyzer()
        {
            /*var parameter = new MeCabParam();*/
            tagger = MeCabTagger.Create();
        }

        public List<JapaneseLexeme> Analyze(string text)
        {
            var nodes = tagger.ParseToNodes(text);
            var lexemes = new List<JapaneseLexeme>();
            foreach (var node in nodes)
            {
                var features = node.Feature.Split(',');
                try
                {
                    //Some interesting thins are being ignored here. Like, it says the conjugation form and type.
                    var lexeme = new JapaneseLexeme(node.Surface, features[0], features[6]);
                    lexemes.Add(lexeme);
                } catch (Exception e)
                {
                    Console.WriteLine($"Error parsing node: {node.Surface} {node.Feature}");
                    Console.WriteLine(e.Message);
                }
            }
            return lexemes;
        }

        public static GrammaticalCategory ParseToCategory(string category)
        {
            switch (category)
            {
                case "名詞":
                    return GrammaticalCategory.Noun;
                case "動詞":
                    return GrammaticalCategory.Verb;
                case "形容詞":
                    return GrammaticalCategory.Adjective;
                case "副詞":
                    return GrammaticalCategory.Adverb;
                case "助詞":
                    return GrammaticalCategory.Particle;
                case "助動詞":
                    return GrammaticalCategory.AuxiliaryVerb;
                case "接続詞":
                    return GrammaticalCategory.Conjunction;
                case "感動詞":
                    return GrammaticalCategory.Interjection;
                case "記号":
                    return GrammaticalCategory.Symbol;
                case "フィラー":
                    return GrammaticalCategory.Filler;
                case "その他":
                    return GrammaticalCategory.Other;
                default:
                    return GrammaticalCategory.Unknown;
            }
        }
    }
}
