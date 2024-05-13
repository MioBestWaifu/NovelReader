using Maria.Common.Communication;
using Maria.Common.Communication.Commanding;
using Maria.Services.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese
{
   
    internal class JapaneseTranslator
    {
        private static string pathToData = @"D:\Programs\Data\JMdict_e\";
        //Remember to start it with an apropriate size
        private ConcurrentDictionary<string, ConversionEntry> conversionTable = [];
        private JapaneseAnalyzer analyzer;
        public static JapaneseTranslator? Instance { get; private set; }
        private JapaneseTranslator() { }

        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception("Already initialized"); //Should be a custom exception
            Instance = new JapaneseTranslator();
            Instance.analyzer = new JapaneseAnalyzer();
            Instance.conversionTable = JapaneseDictionaryLoader.LoadConversionTable();
        }
        
        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.

        //Glosses missing in Extension. Why?
        public string Translate(Command command)
        {
            List<ConversionEntry> conversionEntries = new List<ConversionEntry>();
            if (conversionTable.TryGetValue(command.Options["term"],out ConversionEntry? entry))
            {
               conversionEntries.Add(entry);
            } else {
                List<JapaneseLexeme> lexemes = analyzer.Analyze(command.Options["term"]);
                if (lexemes.Count == 0)
                {
                    Console.WriteLine("No lexemes found");
                    return "Fail";
                }
                GrammaticalCategory[] signicantCategories = new GrammaticalCategory[] { GrammaticalCategory.Noun, GrammaticalCategory.Verb, GrammaticalCategory.Adjective, GrammaticalCategory.Adverb };

                //These relations should be stored somewhere
                foreach (var lexeme in lexemes.Where(x => signicantCategories.Contains(x.Category)))
                {
                    if (conversionTable.TryGetValue(lexeme.BaseForm, out entry))
                    {
                        conversionEntries.Add(entry);
                    }
                }
            }

            //Should be concurrent
            List<EdrdgEntry> dictionaryEntries = new List<EdrdgEntry>();
            foreach (var conversionEntry in conversionEntries)
            {
                dictionaryEntries.Add(JapaneseDictionaryLoader.LoadEntry(conversionEntry.File, conversionEntry.Offset));
            }

            return JsonSerializer.Serialize(dictionaryEntries,CommandServer.jsonOptions);
        }

        public static void Dispose()
        {
            Instance = null;
        }
    }
}
