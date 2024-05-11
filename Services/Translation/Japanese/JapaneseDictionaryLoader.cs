using Maria.Common.Communication;
using Maria.Services.Translation.Japanese.Edrdg;
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
    internal static class JapaneseDictionaryLoader
    {
        //TODO: Those constants shold all be centralized somewhere.
        private static string pathToData = @"Data\Japanese\";
        private static string pathToEdrdg = pathToData + @"EDRDG\";
        private static string pathToOriginalJmdict = pathToEdrdg + @"JMdict_e.xml";
        private static string pathToConvertedJmdict = pathToData + @"JMdict\";
        private static string pathToConversionTable = pathToData + @"ConversionTable.json";

        public static ConcurrentDictionary<string, ConversionEntry> LoadConversionTable()
        {
            string json = File.ReadAllText(pathToConversionTable);
            List<ConversionEntry> conversionEntries = JsonSerializer.Deserialize<List<ConversionEntry>>(json,CommandServer.jsonOptions)!;
            ConcurrentDictionary<string, ConversionEntry> toReturn = new ConcurrentDictionary<string, ConversionEntry>();
            Parallel.ForEach(conversionEntries, entry =>
            {
                toReturn.TryAdd(entry.Key, entry);
            });
            return toReturn;
        }
        
        //Should determine the source (jmdict, names dict) once those othe databases are implemented.
        public static EdrdgEntry LoadEntry(int file, int offset)
        {
            string json = File.ReadAllText($"{pathToConvertedJmdict}{file}.json");
            return JsonSerializer.Deserialize<List<EdrdgEntry>>(json, CommandServer.jsonOptions)![offset];
        }
    }
}
