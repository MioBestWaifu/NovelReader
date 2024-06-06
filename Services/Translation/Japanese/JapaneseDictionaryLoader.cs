using Maria.Commons.Recordkeeping;
using System.Reflection;

namespace Maria.Translation.Japanese
{
    internal static class JapaneseDictionaryLoader
    {
        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public static List<ConversionEntry> LoadPossibleEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            string resourceName = $"Maria.Translation.JMDict.{file}.bin";

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

            return Serializer.DeserializeBytes<List<List<ConversionEntry>>>(data)![offset];
        }

    }
}
