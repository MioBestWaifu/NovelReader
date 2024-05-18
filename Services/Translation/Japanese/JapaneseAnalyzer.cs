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
            //<MeCabUseDefaultDictionary>False</MeCabUseDefaultDictionary>
            var parameter = new MeCabParam();
            parameter.DicDir = Constants.Paths.ToUnidic;
            tagger = MeCabTagger.Create(parameter);
        }

        /// <summary>
        /// 
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
                        var lexeme = new JapaneseLexeme(node.Surface, features[0], features[7]);
                        lexemes.Add(lexeme);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error parsing node: {node.Surface} {node.Feature}");
                        Console.WriteLine(e.Message);
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
                "その他" => GrammaticalCategory.Other,
                _ => GrammaticalCategory.Unknown,
            };
        }
    }
}
