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
        private ConcurrentDictionary<string, EdrdgEntry> conversionTable = [];
        private JapaneseAnalyzer analyzer;
        public static JapaneseTranslator? Instance { get; private set; }
        private JapaneseTranslator() { }

        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception("Already initialized"); //Should be a custom exception
            Instance = new JapaneseTranslator();
        }
        
        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.
        public string Translate(Command command)
        {
            if (conversionTable.TryGetValue(command.Options["term"],out EdrdgEntry? entry))
            {
                //Cast to array because the client expects an array
                return JsonSerializer.Serialize(new EdrdgEntry[] { entry },CommandServer.jsonOptions);
            }
            List<JapaneseLexeme> lexemes = analyzer.Analyze(command.Options["term"]);
            if (lexemes.Count == 0)
            {
                Console.WriteLine("No lexemes found");
                return "Fail";
            }
            GrammaticalCategory[] signicantCategories = new GrammaticalCategory[] { GrammaticalCategory.Noun, GrammaticalCategory.Verb, GrammaticalCategory.Adjective, GrammaticalCategory.Adverb };
            //These relations should be stored somewhere
            List<EdrdgEntry> lexemeBasedEntries = new List<EdrdgEntry>();
            foreach (var lexeme in lexemes.Where(x => signicantCategories.Contains(x.Category)))
            {
                if (conversionTable.TryGetValue(lexeme.BaseForm, out entry))
                {
                    lexemeBasedEntries.Add(entry);
                }
            }

            if (lexemeBasedEntries.Count > 0)
            {
                return JsonSerializer.Serialize(lexemeBasedEntries, CommandServer.jsonOptions);
            }

            return "Fail";
        }

        public static void Dispose()
        {
            Instance = null;
        }
    }
}
