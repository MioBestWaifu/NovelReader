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
        private static ConcurrentDictionary<string, List<(EdrdgEntry, int)>> LoadOriginalJMdict(string pathToJmdict)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(pathToJmdict, settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, List<(EdrdgEntry, int)>> toReturn = [];
            Parallel.ForEach(elements, element =>
            {
                //Two important things should be done here:
                //1 - it should check if the entry is usually in kana. this info is contained in the dict but ignored. if it is, then kana keys should be added irrespective of the kanji keys.
                //2 - the values should be a list of entries, and the user gets all of them as answers.
                EdrdgEntry entry = new EdrdgEntry(element);
                int baseKanjiPriority = 0;
                if (entry.KanjiElements != null)
                {
                    foreach (var kanjiElement in entry.KanjiElements)
                    {
                        //this should take things such as commonality, archaism, etc, into account.
                        int priority = baseKanjiPriority;
                        List<(EdrdgEntry, int)> entries;
                        if (!toReturn.TryGetValue(kanjiElement.Kanji, out entries))
                        {
                            entries = new List<(EdrdgEntry, int)>();
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
                        List<(EdrdgEntry, int)> entries;
                        if (!toReturn.TryGetValue(readingElement.Reading, out entries))
                        {
                            entries = new List<(EdrdgEntry, int)>();
                            toReturn.TryAdd(readingElement.Reading, entries);
                        }
                        entries.Add((entry, priority));
                    }
                }
            });

            return toReturn;
        }

        //Yes, thats a lot of lists. No, it is not a better to use other structure. It represents file > content > index.
        private static List<List<List<ConversionEntry>>> CreateHashes(string pathToJmdict)
        {
            List<List<List<ConversionEntry>>> jmdictiesBrokenByIndex = new List<List<List<ConversionEntry>>>();

            ConcurrentDictionary<string, List<(EdrdgEntry,int)>> originalJMdict = LoadOriginalJMdict(pathToJmdict);
            for (int i = 0; i < 256; i++)
            {

                jmdictiesBrokenByIndex.Add(new List<List<ConversionEntry>>());
                for (int j = 0; j < 256; j++)
                {
                    jmdictiesBrokenByIndex[i].Add(new List<ConversionEntry>());
                }

            }

            int file = 0;
            SHA256 sha256 = SHA256.Create();
            foreach (var entryList in originalJMdict)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(entryList.Key));
                int number = BitConverter.ToUInt16(hash, 0); // Convert the first two bytes to a number
                foreach (var entry in entryList.Value)
                {
                    jmdictiesBrokenByIndex[number / 256][number % 256].Add(new ConversionEntry(entryList.Key, entry.Item1,entry.Item2));
                }
            }

            return jmdictiesBrokenByIndex;
        }

        public static void CreateJmdict(string pathToOriginalJmdict, string pathToOutput)
        {
            Directory.CreateDirectory(pathToOutput);
            List<List<List<ConversionEntry>>> jmdictiesBrokenByFile = CreateHashes(pathToOriginalJmdict);
            for (int i = 0; i < jmdictiesBrokenByFile.Count; i++)
            {
                byte[] jmdictMsgPack = MessagePackSerializer.Serialize(jmdictiesBrokenByFile[i]);
                File.WriteAllBytes($@"{pathToOutput}{i}.bin", jmdictMsgPack);
            }
        }
    }
}
