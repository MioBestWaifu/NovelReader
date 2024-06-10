using Mio.Translation;
using Mio.Translation.Japanese.Edrdg;
using System.Security.Cryptography;
using System.Text;

namespace Mio.Translation.Japanese
{

    public class JapaneseTranslator
    {

        //This should return something else, a custom type for translations maybe. But that requires rethinking the 
        //command response interface and that will be done later.
        //No need to be async now, may be later.
        public List<EdrdgEntry> Translate(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Term cannot be null or empty");
            }

            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries = JapaneseDictionaryLoader.LoadPossibleEntries(termIndex);

            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list (rarely >6)
            //it doesn't matter.
            //Also, this only takes one entry, because right know there can be only one unique key. This will change.
            ConversionEntry? match = possibleEntries.Find(x => x.Key == term);
            List<EdrdgEntry> dictionaryEntries = new List<EdrdgEntry>();

            if (match is not null)
            {
                dictionaryEntries.Add(match.Value);
                return dictionaryEntries;
            }

            //Revise wether it should return null, throw an exception or return an empty list.
            return [null];
        }
    }
}
