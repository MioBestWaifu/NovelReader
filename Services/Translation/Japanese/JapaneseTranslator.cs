using Maria.Commons.Communication.Commanding;
using Maria.Commons.Recordkeeping;
using Maria.Translation.Japanese.Edrdg;
using System.Security.Cryptography;
using System.Text;

namespace Maria.Translation.Japanese
{

    public class JapaneseTranslator
    {
        private readonly string pathToDictionary;

        public JapaneseTranslator(string pathToDictionary)
        {
            this.pathToDictionary = pathToDictionary;
        }

        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.
        public string Translate(Command command)
        {
            if (!command.Options.TryGetValue("term", out string term))
            {
                return "No term to translate";
            }
            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries = new JapaneseDictionaryLoader(pathToDictionary).LoadPossibleEntries(termIndex);

            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list (rarely >6)
            //it doesn't matter.
            //Also, this only takes one entry, because right know there can be only one unique key. This will change.
            ConversionEntry? match = possibleEntries.Find(x => x.Key == term);
            List<EdrdgEntry> dictionaryEntries = new List<EdrdgEntry>();

            if (match is not null)
            {
                dictionaryEntries.Add(match.Value);
                return Serializer.SerializeToJson(dictionaryEntries);
            }

            return "No match found";
        }

        public string Translate(string term)
        {
            return Translate(new Command()
            {
                Options = { { "term", term } }
            });    
        }
    }
}
