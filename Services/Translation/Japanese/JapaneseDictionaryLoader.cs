using Maria.Commons.Recordkeeping;

namespace Maria.Translation.Japanese
{
    internal class JapaneseDictionaryLoader
    {
        private string pathToDictionary; 

        public JapaneseDictionaryLoader(string pathToDictionary)
        {
            this.pathToDictionary = pathToDictionary;
        }
        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public List<ConversionEntry> LoadPossibleEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            byte[] data = File.ReadAllBytes($"{pathToDictionary}{file}.bin");
            return Serializer.DeserializeBytes<List<List<ConversionEntry>>>(data)![offset];
        }
    }
}
