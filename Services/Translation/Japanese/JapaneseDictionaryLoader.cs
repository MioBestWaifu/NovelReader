using Maria.Commons.Recordkeeping;

namespace Maria.Translation.Japanese
{
    internal static class JapaneseDictionaryLoader
    {
        public static string pathToDictionary; 
        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public static List<ConversionEntry> LoadPossibleEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            byte[] data = File.ReadAllBytes($"{pathToDictionary}{file}.bin");
            return Serializer.DeserializeBytes<List<List<ConversionEntry>>>(data)![offset];
        }
    }
}
