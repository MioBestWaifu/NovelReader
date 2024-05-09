using Maria.Services.Translation.Japanese.Edrdg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese
{
    /// <summary>
    /// After initialized, this becomes a big fucking object because it basically holds multiple dictionaries (the thing, not the class)
    /// in memory. This could, and depending on how large it gets should, be memory-optmized by keeping only high-priority
    /// lexeme in memory and searching for others on the files as needed. However, as of implementation memory is an abundant resource, 
    /// while response time is absolutely critical. As such, for now i am sacrifing memory for responsiviness.
    /// </summary>
    internal class JapaneseTranslator
    {
        private static string pathToData = @"D:\Programs\Data\JMdict_e\";
        //Remember to start it with an apropriate size
        private Dictionary<string, EdrdgEntry> conversionTable = [];
        public static JapaneseTranslator? Instance { get; private set; }
        private JapaneseTranslator() { }

        /// <summary>
        /// This is expensive as fuck
        /// </summary>
        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception(); //Should be a custom exception

            Stopwatch stopwatch = Stopwatch.StartNew();
            Instance = new JapaneseTranslator();
            XDocument jmdict = XDocument.Load(pathToData+ "JMdict_e.xml");
            IEnumerable<XElement> elements = jmdict.Elements("entry");
            foreach (XElement element in elements)
            {
                EdrdgEntry entry = new EdrdgEntry(element);
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElemnt in entry.KanjiElements)
                    {
                        Instance.conversionTable.Add(kanjiElemnt.Kanji,entry);
                    }
                } else
                {
                    foreach(var readingElemnt in entry.ReadingElements)
                    {
                        Instance.conversionTable.Add(readingElemnt.Reading, entry);
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Time to load: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"Number of elements: {Instance.conversionTable.Count}");
        }

        public static void Dispose()
        {
            Instance = null;
        }
    }
}
