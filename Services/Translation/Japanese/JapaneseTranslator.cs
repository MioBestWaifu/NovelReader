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
    /// <summary>
    /// After initialized, this becomes a big fucking object, 600MB big, because it basically holds multiple dictionaries (the thing AND the class)
    /// in memory. This could, and depending on how large it gets should, be memory-optmized by keeping only high-priority
    /// lexeme in memory and searching for others on the files as needed. However, as of implementation memory is an abundant resource, 
    /// while response time is absolutely critical. As such, for now i am sacrificing memory for responsiviness.
    /// </summary>
    internal class JapaneseTranslator
    {
        private static string pathToData = @"D:\Programs\Data\JMdict_e\";
        //Remember to start it with an apropriate size
        private ConcurrentDictionary<string, EdrdgEntry> conversionTable = [];
        private JapaneseAnalyzer analyzer;
        public static JapaneseTranslator? Instance { get; private set; }
        private JapaneseTranslator() { }

        /// <summary>
        /// This is expensive as fuck, like 7 seconds expensive.
        /// </summary>
        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception("Already initialized"); //Should be a custom exception

            Stopwatch stopwatch = Stopwatch.StartNew();
            Instance = new JapaneseTranslator();
            Instance.analyzer = new JapaneseAnalyzer();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(pathToData+ "JMdict_e.xml", settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            Parallel.ForEach(elements, element =>
            {
                //Two important things should be done here:
                //1 - it should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
                //2 - the values should be a list of entries, and the user gets all of them as answers.
                EdrdgEntry entry = new EdrdgEntry(element);
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElemnt in entry.KanjiElements)
                    {
                        try
                        {
                            if (!Instance.conversionTable.TryAdd(kanjiElemnt.Kanji, entry))
                                argumentExisting++;
                        }
                        catch (ArgumentException)
                        {
                            //This is a very common exception, as many entries have the same kanji. It is not an error.
                            //It will be dealt with eventually.
                            argumentExisting++;
                        }
                    }
                }
                else
                {
                    foreach (var readingElemnt in entry.ReadingElements)
                    {
                        try
                        {
                            if (!Instance.conversionTable.TryAdd(readingElemnt.Reading, entry))
                                argumentExisting++;
                        }
                        catch (ArgumentException)
                        {
                            //This is a very common exception, as many entries have the same kana. It is not an error.
                            //It will be dealt with eventually.
                            argumentExisting++;
                        }
                    }
                }
            });
            stopwatch.Stop();
            Console.WriteLine("Time to load: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"Number of elements: {Instance.conversionTable.Count}");
            Console.WriteLine($"Number of times the key already existed: {argumentExisting}");
        }
        
        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.
        public string Translate(Command command)
        {
            if (conversionTable.TryGetValue(command.Options["term"],out EdrdgEntry? entry))
            {
                return JsonSerializer.Serialize(entry,CommandServer.jsonOptions);
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
