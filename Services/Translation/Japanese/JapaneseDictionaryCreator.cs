using Maria.Services.Translation.Japanese.Edrdg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Text.Json;
using Maria.Common.Communication;
using MessagePack;
using System.Globalization;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;

namespace Maria.Services.Translation.Japanese
{
    /// <summary>
    /// Takes the EDRDG database and creates files with it to be read by Maria. It is not supposed to be run in the final product.
    /// Is also not supposed to be run all the time in development, only when the database is updated.
    /// </summary>
    internal static class JapaneseDictionaryCreator
    {

        private static ConcurrentDictionary<string, EdrdgEntry> LoadOriginalJMdict()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.MaxCharactersFromEntities = 0;
            settings.MaxCharactersInDocument = 0;
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(Constants.Paths.ToEdrdg+ "JMdict_e.xml", settings);
            XDocument jmdict = XDocument.Load(reader);
            IEnumerable<XElement> elements = jmdict.Element("JMdict")!.Elements("entry");
            int argumentExisting = 0;
            ConcurrentDictionary<string, EdrdgEntry> toReturn = new ConcurrentDictionary<string, EdrdgEntry>();
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
                            if (!toReturn.TryAdd(kanjiElemnt.Kanji, entry))
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
                            if (!toReturn.TryAdd(readingElemnt.Reading, entry))
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

            return toReturn;
        }

        //Yes, thats a lot of lists. No, it is not a better to use other structure. It represents file > content > index.
        private static List<List<List<ConversionEntry>>> CreateHashes()
        {
            List<List<List<ConversionEntry>>> jmdictiesBrokenByIndex = new List<List<List<ConversionEntry>>>();

            ConcurrentDictionary<string, EdrdgEntry> originalJMdict = LoadOriginalJMdict();

            for (int i = 0; i < 256; i++)
            {
                
                jmdictiesBrokenByIndex.Add(new List<List<ConversionEntry>>());
                for (int j = 0; j< 256; j++)
                {
                    jmdictiesBrokenByIndex[i].Add(new List<ConversionEntry>());
                }
               
            }

            int file = 0;
            SHA256 sha256 = SHA256.Create();
            foreach (var entry in originalJMdict)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(entry.Key));
                int number = BitConverter.ToUInt16(hash, 0); // Convert the first two bytes to a number

                jmdictiesBrokenByIndex[number / 256][number%256].Add(new ConversionEntry(entry.Key, entry.Value));
            }

            return jmdictiesBrokenByIndex;
        }

        public static void CreateDictionary()
        {
            Directory.CreateDirectory(Constants.Paths.ToConvertedDictionary);
            List<List<List<ConversionEntry>>> jmdictiesBrokenByFile = CreateHashes();
            for (int i = 0; i < jmdictiesBrokenByFile.Count; i++)
            {
                byte[] jmdictMsgPack = MessagePackSerializer.Serialize(jmdictiesBrokenByFile[i]);
                File.WriteAllBytes($@"{Constants.Paths.ToConvertedDictionary}{i}.bin", jmdictMsgPack);
            }
        }
    }
}
