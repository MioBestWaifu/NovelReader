using Maria.Common.Communication;
using Maria.Common.Communication.Commanding;
using Maria.Services.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
        private JapaneseAnalyzer analyzer;
        public static JapaneseTranslator? Instance { get; private set; }
        private JapaneseTranslator() { }

        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception("Already initialized"); //Should be a custom exception
            Instance = new JapaneseTranslator();
            Instance.analyzer = new JapaneseAnalyzer();
        }

        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.
        public string Translate(Command command)
        {
            if (!command.Options.TryGetValue("term", out string term))
            {
                return "No term to translate";
            }
            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries = JapaneseDictionaryLoader.LoadPossibleEntries(termIndex);

            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list (rarely >6)
            //it doesn't matter.
            ConversionEntry? match = possibleEntries.Find(x => x.Key == term);
            List<EdrdgEntry> dictionaryEntries = new List<EdrdgEntry>();

            if (match is not null)
            {
                dictionaryEntries.Add(match.Value);
            }
            else
            {
                List<JapaneseLexeme> lexemes = analyzer.Analyze(term);
                GrammaticalCategory[] signicantCategories = { GrammaticalCategory.Noun, GrammaticalCategory.Verb, GrammaticalCategory.Adjective, GrammaticalCategory.Adverb };

                //These relations should be stored somewhere
                //Also, this is not efficient. This block could be started in parallel from the previous one, and its results used
                //if no mataches are found. Besides, it should be Parallel.ForEach.
                foreach (var lexeme in lexemes.Where(x => signicantCategories.Contains(x.Category)))
                {
                    termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(lexeme.BaseForm));
                    termIndex = BitConverter.ToUInt16(termHash, 0);
                    possibleEntries = JapaneseDictionaryLoader.LoadPossibleEntries(termIndex);
                    match = possibleEntries.Find(x => x.Key == lexeme.BaseForm);
                    if (match is not null)
                    {
                        dictionaryEntries.Add(match.Value);
                    }
                }
            }

            return JsonSerializer.Serialize(dictionaryEntries, CommandServer.jsonOptions);
        }

        public static void Dispose()
        {
            Instance = null;
        }
    }
}
