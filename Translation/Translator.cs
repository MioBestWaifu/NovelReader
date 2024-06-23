using Mio.Translation.Dictionaries;
using Mio.Translation.Entries;
using MyNihongo.KanaConverter;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mio.Translation
{

    public class Translator
    {

        /// <summary>
        /// To translate into from the JMdict
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<JmdictEntry> TranslateWord(string term)
        {
            List<ConversionEntry>? matches = FindPossibleEntries(term, DatabaseDictionary.JMdict);
            if (matches is not null)
            {
                List<JmdictEntry> dictionaryEntries = new List<JmdictEntry>();
                foreach (var item in matches)
                {
                    dictionaryEntries.Add((JmdictEntry)item.Value);
                }
                return dictionaryEntries;
            }

            //Revise wether it should return null, throw an exception or return an empty list.
            return [null];
        }

        /// <summary>
        /// To translate from the JMnedict
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public NamedictEntry TranslateName(string term)
        {
            List<ConversionEntry>? matches = FindPossibleEntries(term, DatabaseDictionary.JMnedict);

            if (matches is not null)
            {
                return (NamedictEntry)matches[0].Value;
            }

            //Revise wether it should return null, throw an exception or return an empty list.
            return null;
        }

        private List<ConversionEntry>? FindPossibleEntries(string term, DatabaseDictionary dictionary)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Term cannot be null or empty");
            }

            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries;
            if (dictionary == DatabaseDictionary.JMdict)
            {
                possibleEntries = DictionaryLoader.LoadPossibleJmdictEntries(termIndex);
            }
            else if (dictionary == DatabaseDictionary.JMnedict)
            {
                possibleEntries = DictionaryLoader.LoadPossibleJmnedictEntries(termIndex);
            }
            else
            {
                throw new ArgumentException("Dictionary not supported");
            }
            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list
            //it doesn't matter. The conversions might matter, tough.
            List<ConversionEntry>? matches = possibleEntries.FindAll(x => x.Key == term).OrderBy(x => x.Priority).ToList();
            return matches;
        }

        public KanjidicEntry TranslateKanji(char kanji)
        {
            int code = Analyzer.DetermineKanjiNumber(kanji);
            return DictionaryLoader.LoadKanjiEntry(code);
        }

        public string TranslateKana(string kana)
        {
            return kana.ToRomaji();
        }
    }
}
