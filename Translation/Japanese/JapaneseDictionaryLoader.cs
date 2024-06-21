using MessagePack;
using System.Reflection;

namespace Mio.Translation.Japanese
{
    internal static class JapaneseDictionaryLoader
    {
        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public static List<ConversionEntry> LoadPossibleJmdictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMDict.{file}.bin";

            byte[] data;
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyToAsync(memoryStream).Wait();
                    data = memoryStream.ToArray();
                }
            }

            //This one call is upwards of 50% of chapter load time. Optmization so far not needed, if needed 
            //use a cache with this thing.
            return MessagePackSerializer.Deserialize<List<List<ConversionEntry>>>(data)![offset];
        }

        public static List<ConversionEntry> LoadPossibleJmnedictEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Mio.Translation.JMnedict.{file}.bin";

            byte[] data;
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyToAsync(memoryStream).Wait();
                    data = memoryStream.ToArray();
                }
            }

            //This one call is upwards of 50% of chapter load time. Optmization so far not needed, if needed 
            //use a cache with this thing.
            return MessagePackSerializer.Deserialize<List<List<ConversionEntry>>>(data)![offset];
        }

    }
}
