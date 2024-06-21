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
        //TODO refactor this duplicated logic
        private static ConcurrentDictionary<string, List<(JmdictEntry, int)>> LoadOriginalJMdict(string pathToJmdict)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(pathToJmdict, settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, List<(JmdictEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                //Two important things should be done here:
                //1 - it should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
                //2 - the values should be a list of entries, and the user gets all of them as answers.
                JmdictEntry entry = new JmdictEntry(element);
                int baseKanjiPriority = 0;
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElement in entry.KanjiElements)
                    {
                        //this should take things such as commonality, archaism, etc, into account.
                        int priority = baseKanjiPriority;
                        List<(JmdictEntry, int)> entries;
                        if (!toReturn.TryGetValue(kanjiElement.Kanji, out entries))
                        {
                            entries = new List<(JmdictEntry, int)>();
                            toReturn.TryAdd(kanjiElement.Kanji, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
                if (entry.ReadingElements != null)
                {
                    int baseReadingPriority = entry.KanjiElements == null || entry.KanjiElements.Count == 0 ? 0 : 10;
                    foreach (var readingElement in entry.ReadingElements)
                    {
                        //again, should take things such as commonality, archaism, etc, into account.
                        int priority = baseReadingPriority;
                        List<(JmdictEntry, int)> entries;
                        if (!toReturn.TryGetValue(readingElement.Reading, out entries))
                        {
                            entries = new List<(JmdictEntry, int)>();
                            toReturn.TryAdd(readingElement.Reading, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
            });

            return toReturn;
        }
        private static ConcurrentDictionary<string, List<(NameEntry, int)>> LoadOriginalJMnedict(string pathToJmnedict)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(pathToJmnedict, settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMnedict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, List<(NameEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                //Two important things should be done here:
                //1 - it should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
                NameEntry entry = new NameEntry(element);
                int baseKanjiPriority = 0;
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElement in entry.KanjiElements)
                    {
                        //this should take things such as commonality, archaism, etc, into account.
                        int priority = baseKanjiPriority;
                        List<(NameEntry, int)> entries;
                        if (!toReturn.TryGetValue(kanjiElement.Kanji, out entries))
                        {
                            entries = new List<(NameEntry, int)>();
                            toReturn.TryAdd(kanjiElement.Kanji, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
                if (entry.ReadingElements != null)
                {
                    int baseReadingPriority = entry.KanjiElements == null || entry.KanjiElements.Count == 0 ? 0 : 10;
                    foreach (var readingElement in entry.ReadingElements)
                    {
                        //again, should take things such as commonality, archaism, etc, into account.
                        int priority = baseReadingPriority;
                        List<(NameEntry, int)> entries;
                        if (!toReturn.TryGetValue(readingElement.Reading, out entries))
                        {
                            entries = new List<(NameEntry, int)>();
                            toReturn.TryAdd(readingElement.Reading, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
                foreach (var translationElement in entry.TranslationElements)
                {
                    //Should this have priority too?
                    List<(NameEntry, int)> entries;
                    if (!toReturn.TryGetValue(translationElement.Translation, out entries))
                    {
                        entries = new List<(NameEntry, int)>();
                        toReturn.TryAdd(translationElement.Translation, entries);
                    }
                    entries.Add((entry, 0));
                }
            });

            return toReturn;
        }
        private static ConcurrentDictionary<string, List<(JmdictEntry, int)>> LoadOriginalKanjidic(string pathToKanjidic)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(pathToKanjidic, settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, List<(JmdictEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                //Two important things should be done here:
                //1 - it should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
                //2 - the values should be a list of entries, and the user gets all of them as answers.
                JmdictEntry entry = new JmdictEntry(element);
                int baseKanjiPriority = 0;
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElement in entry.KanjiElements)
                    {
                        //this should take things such as commonality, archaism, etc, into account.
                        int priority = baseKanjiPriority;
                        List<(JmdictEntry, int)> entries;
                        if (!toReturn.TryGetValue(kanjiElement.Kanji, out entries))
                        {
                            entries = new List<(JmdictEntry, int)>();
                            toReturn.TryAdd(kanjiElement.Kanji, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
                if (entry.ReadingElements != null)
                {
                    int baseReadingPriority = entry.KanjiElements == null || entry.KanjiElements.Count == 0 ? 0 : 10;
                    foreach (var readingElement in entry.ReadingElements)
                    {
                        //again, should take things such as commonality, archaism, etc, into account.
                        int priority = baseReadingPriority;
                        List<(JmdictEntry, int)> entries;
                        if (!toReturn.TryGetValue(readingElement.Reading, out entries))
                        {
                            entries = new List<(JmdictEntry, int)>();
                            toReturn.TryAdd(readingElement.Reading, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
            });

            return toReturn;
        }

        //Yes, thats a lot of lists. No, it is not a better to use other structure. It represents file > content > index.
        private static List<List<List<ConversionEntry>>> CreateJmdictHashes(string pathToDict)
        {
            List<List<List<ConversionEntry>>> entriesBrokenByIndex = new List<List<List<ConversionEntry>>>();

            ConcurrentDictionary<string, List<(JmdictEntry, int)>> originalDict = LoadOriginalJMdict(pathToDict);
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

        private static List<List<List<ConversionEntry>>> CreateJmnedictHashes(string pathToDict)
        {
            List<List<List<ConversionEntry>>> entriesBrokenByIndex = new List<List<List<ConversionEntry>>>();

            ConcurrentDictionary<string, List<(NameEntry, int)>> originalDict = LoadOriginalJMnedict(pathToDict);
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

        public static void CreateDictionary(string pathToOriginal, string pathToOutput, EdrdgDictionary dictionary)
        {
            Directory.CreateDirectory(pathToOutput);
            switch (dictionary)
            {
                case EdrdgDictionary.JMdict:
                    List<List<List<ConversionEntry>>> entriesBrokenByFile = CreateJmdictHashes(pathToOriginal);
                    for (int i = 0; i < entriesBrokenByFile.Count; i++)
                    {
                        byte[] jmdictMsgPack = MessagePackSerializer.Serialize(entriesBrokenByFile[i]);
                        File.WriteAllBytes($@"{pathToOutput}{i}.bin", jmdictMsgPack);
                    }
                    break;
                case EdrdgDictionary.JMnedict:
                    List<List<List<ConversionEntry>>> jmnedictEntriesBrokenByFile = CreateJmnedictHashes(pathToOriginal);
                    for (int i = 0; i < jmnedictEntriesBrokenByFile.Count; i++)
                    {
                        byte[] jmnedictMsgPack = MessagePackSerializer.Serialize(jmnedictEntriesBrokenByFile[i]);
                        File.WriteAllBytes($@"{pathToOutput}{i}.bin", jmnedictMsgPack);
                    }
                    break;
                
            }
        }
    }
}
