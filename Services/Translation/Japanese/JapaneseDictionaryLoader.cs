using Maria.Common.Communication;
using Maria.Services.Translation.Japanese.Edrdg;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Services.Translation.Japanese
{
    //Should be on Recordkeeping. It may have japanese in the name, but it's function is dealing wih records.
    internal static class JapaneseDictionaryLoader
    {
        //TODO: Those constants shold all be centralized somewhere.
        private static string pathToData = @"Data\Japanese\";
        private static string pathToEdrdg = pathToData + @"EDRDG\";
        private static string pathToOriginalJmdict = pathToEdrdg + @"JMdict_e.xml";
        private static string pathToConvertedJmdict = pathToData + @"JMdict\";
        private static string pathToConversionTable = pathToData + @"ConversionTable.bin";

        public static ConcurrentDictionary<string, ConversionEntry> LoadConversionTable()
        {
            byte[] data = File.ReadAllBytes(pathToConversionTable);
            List<ConversionEntry> conversionEntries = MessagePackSerializer.Deserialize<List<ConversionEntry>>(data)!;
            ConcurrentDictionary<string, ConversionEntry> toReturn = new ConcurrentDictionary<string, ConversionEntry>();
            Parallel.ForEach(conversionEntries, entry =>
            {
                toReturn.TryAdd(entry.Key, entry);
            });
            return toReturn;
        }
        
        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public static List<HashedEntry> LoadPossibleEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            byte[] data = File.ReadAllBytes($"{pathToConvertedJmdict}{file}.bin");
            return MessagePackSerializer.Deserialize<List<List<HashedEntry>>>(data)![offset];
        }
    }
}
