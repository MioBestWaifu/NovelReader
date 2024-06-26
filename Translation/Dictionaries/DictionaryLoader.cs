﻿using MessagePack;
using Mio.Translation.Entries;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mio.Translation.Dictionaries
{
    internal static class DictionaryLoader
    {
        //The deserializer calls here are upwards of 60% of chapter load time on Windows. Optmization so far not needed, if needed 
        //use a cache with this thing.
        public static List<ConversionEntry> LoadPossibleJmdictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMDict.{file}.bin";

            byte[] data = LoadResourceToBytes(resourceName);

            return MessagePackSerializer.Deserialize<List<List<ConversionEntry>>>(data)![offset];
        }

        public static List<ConversionEntry> LoadPossibleJmnedictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMnedict.{file}.bin";

            byte[] data = LoadResourceToBytes(resourceName);

            return MessagePackSerializer.Deserialize<List<List<ConversionEntry>>>(data)![offset];
        }

        public static KanjidicEntry LoadKanjiEntry(int index)
        {
            int file = Math.DivRem(index, 1000, out int offset);
            string resourceName = $"Mio.Translation.Kanjidic.{file}.bin";
            byte[] data = LoadResourceToBytes(resourceName);

            return MessagePackSerializer.Deserialize<KanjidicEntry[]>(data)![offset];
        }

        private static byte[] LoadResourceToBytes(string resourceName)
        {
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyToAsync(memoryStream).Wait();
                    return memoryStream.ToArray();
                }
            }
        }

    }
}
