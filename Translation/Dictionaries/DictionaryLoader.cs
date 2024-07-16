using MessagePack;
using Mio.Translation.Entries;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mio.Translation.Dictionaries
{
    internal static class DictionaryLoader
    {
        public static async Task<List<ConversionEntry>> LoadPossibleJmdictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMDict.{file}.bin";

            byte[] data = await LoadResourceToBytes(resourceName);

            var deserializedData = await MessagePackSerializer.DeserializeAsync<List<List<ConversionEntry>>>(new MemoryStream(data));
            return deserializedData![offset];
        }

        public static async Task<List<ConversionEntry>> LoadPossibleJmnedictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMnedict.{file}.bin";

            byte[] data = await LoadResourceToBytes(resourceName);

            var deserializedData = await MessagePackSerializer.DeserializeAsync<List<List<ConversionEntry>>>(new MemoryStream(data));
            return deserializedData![offset];
        }

        public static async Task<KanjidicEntry> LoadKanjiEntry(int index)
        {
            int file = Math.DivRem(index, 1000, out int offset);
            string resourceName = $"Mio.Translation.Kanjidic.{file}.bin";
            byte[] data = await LoadResourceToBytes(resourceName);

            var deserializedData = await MessagePackSerializer.DeserializeAsync<List<KanjidicEntry>>(new MemoryStream(data));
            return deserializedData![offset];
        }

        private static async Task<byte[]> LoadResourceToBytes(string resourceName)
        {
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await resourceStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

    }
}
