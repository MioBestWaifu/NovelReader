using System.Collections.Concurrent;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using MessagePack;
using System.Security.Cryptography;
using Mio.Translation;
using Mio.Translation.Japanese.Edrdg;

namespace Mio.Translation.Japanese
{
    /// <summary>
    /// Takes the EDRDG database and creates translation files. It is not supposed to be run in the final product.
    /// Is also not supposed to be run all the time in development, only when the database is updated.
    /// </summary>
    public static class JapaneseDictionaryCreator
    {
        private static XDocument LoadOriginal(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(path, settings);
            return XDocument.Load(reader);
        }

        private static void AddKanjiAndReadingKeys<T>(T entry, ConcurrentDictionary<string, List<(T, int)>> dict) where T : EdrdgEntry
        {
            //Should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
            if (entry.KanjiElements != null)
            {
                foreach (var kanjiElement in entry.KanjiElements)
                {
                    List<(T, int)> entries;
                    if (!dict.TryGetValue(kanjiElement.Kanji, out entries))
                    {
                        entries = new List<(T, int)>();
                        dict.TryAdd(kanjiElement.Kanji, entries);
                    }
                    entries.Add((entry, kanjiElement.Priority));
                }
            }
            if (entry.ReadingElements != null)
            {
                //There is probably a better sync mechanism, but this is good enough because this thing wont run often.
                lock (entry.readingElementsLock)
                {
                    foreach (var readingElement in entry.ReadingElements)
                    {
                        List<(T, int)> entries;
                        if (!dict.TryGetValue(readingElement.Reading, out entries))
                        {
                            entries = new List<(T, int)>();
                            dict.TryAdd(readingElement.Reading, entries);
                        }
                        entries.Add((entry, readingElement.Priority));
                    }
                }
            }
        }

        private static ConcurrentDictionary<string, List<(JmdictEntry, int)>> LoadOriginalJMdict(string pathToJmdict)
        {
            XDocument jmdict = LoadOriginal(pathToJmdict);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, List<(JmdictEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                JmdictEntry entry = new JmdictEntry(element);
                AddKanjiAndReadingKeys(entry, toReturn);
            });

            return toReturn;
        }
        private static ConcurrentDictionary<string, List<(NameEntry, int)>> LoadOriginalJMnedict(string pathToJmnedict)
        {
            XDocument jmnedict = LoadOriginal(pathToJmnedict);
            IEnumerable<XElement> elements = jmnedict.Element("JMnedict")!.Elements("entry");
            ConcurrentDictionary<string, List<(NameEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                NameEntry entry = new NameEntry(element);
                //This is to optimize storage, serialization, deserialization and display.
                //The logic here is that if a name has the same kanji, it is the same name, but Jmnedict has multiple entries for it.
                if (entry.KanjiElements.Count > 0 && toReturn.TryGetValue(entry.KanjiElements[0].Kanji,out var list))
                {
                    list[0].Item1.Append(entry);
                    return;
                }
                AddKanjiAndReadingKeys(entry, toReturn);
            });

            return toReturn;
        }

        //Yes, thats a lot of lists. No, it is not a better to use other structure. It represents file > content > index.
        private static List<List<List<ConversionEntry>>> CreateHashes<T>(ConcurrentDictionary<string,List<(T,int)>> originalDict) where T: EdrdgEntry
        {
            List<List<List<ConversionEntry>>> entriesBrokenByIndex = new List<List<List<ConversionEntry>>>();
            for (int i = 0; i < 256; i++)
            {

                entriesBrokenByIndex.Add(new List<List<ConversionEntry>>());
                for (int j = 0; j < 256; j++)
                {
                    entriesBrokenByIndex[i].Add(new List<ConversionEntry>());
                }

            }

            int file = 0;
            SHA256 sha256 = SHA256.Create();
            foreach (var entryList in originalDict)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(entryList.Key));
                int number = BitConverter.ToUInt16(hash, 0); // Convert the first two bytes to a number
                foreach (var entry in entryList.Value)
                {
                    entriesBrokenByIndex[number / 256][number % 256].Add(new ConversionEntry(entryList.Key, entry.Item1, entry.Item2));
                }
            }

            return entriesBrokenByIndex;
        }

        private static KanjiEntry[] CreateKanjiEntries(string pathToKanjidict)
        {
            XDocument kanjidict = LoadOriginal(pathToKanjidict);
            IEnumerable<XElement> elements = kanjidict.Element("kanjidic2")!.Elements("character");
            //The largest possible value got from DetermineKanjiNumber is supposed be 79-odd thousand.
            KanjiEntry[] entries = new KanjiEntry[80000];
            int exceptionCount = 0;
            foreach(XElement element in elements)
            {
                //There is one kanji here making problem: 𠀋. Something to do with bytes and encoding.
                //There may be others. Seem to be unusual kanji at the end of kanjidic with incomplete information.
                try
                {
                    KanjiEntry entry = new KanjiEntry(element);
                    Console.WriteLine(entry.Literal);
                    int code = JapaneseAnalyzer.DetermineKanjiNumber(entry.Literal);
                    entries[code] = entry;
                } catch (Exception e)
                {
                    exceptionCount++;
                    Console.WriteLine(exceptionCount);
                }
            }
            return entries;
        }

        public static void CreateDictionary(string pathToOriginal, string pathToOutput, EdrdgDictionary dictionary)
        {
            Directory.CreateDirectory(pathToOutput);
            switch (dictionary)
            {
                case EdrdgDictionary.JMdict:
                    List<List<List<ConversionEntry>>> entriesBrokenByFile = 
                        CreateHashes(LoadOriginalJMdict(pathToOriginal));
                    for (int i = 0; i < entriesBrokenByFile.Count; i++)
                    {
                        byte[] jmdictMsgPack = MessagePackSerializer.Serialize(entriesBrokenByFile[i]);
                        File.WriteAllBytes($@"{pathToOutput}{i}.bin", jmdictMsgPack);
                    }
                    break;
                case EdrdgDictionary.JMnedict:
                    List<List<List<ConversionEntry>>> jmnedictEntriesBrokenByFile = 
                        CreateHashes(LoadOriginalJMnedict(pathToOriginal));
                    for (int i = 0; i < jmnedictEntriesBrokenByFile.Count; i++)
                    {
                        byte[] jmnedictMsgPack = MessagePackSerializer.Serialize(jmnedictEntriesBrokenByFile[i]);
                        File.WriteAllBytes($@"{pathToOutput}{i}.bin", jmnedictMsgPack);
                    }
                    break;
                case EdrdgDictionary.Kanjidic:
                    KanjiEntry[] kanjiEntries = CreateKanjiEntries(pathToOriginal);
                    for (int i = 0; i < 79; i++)
                    {
                        int startRange = i * 1000;
                        int endRange = startRange + 999;
                        byte[] kanjidicMsgPack = MessagePackSerializer.Serialize(kanjiEntries[startRange..endRange]);
                        File.WriteAllBytes($@"{pathToOutput}{i}.bin", kanjidicMsgPack);
                    }
                    break;
                
            }
        }
    }
}
