using Mio.Translation;
using Mio.Translation.Japanese.Edrdg;
using MyNihongo.KanaConverter;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mio.Translation.Japanese
{

    public class JapaneseTranslator
    {

        /// <summary>
        /// To translate into from the JMdict
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<JmdictEntry> TranslateWord(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Term cannot be null or empty");
            }

            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries = JapaneseDictionaryLoader.LoadPossibleJmdictEntries(termIndex);

            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list
            //it doesn't matter. The conversions might matter, tough.
            List<ConversionEntry>? matches = possibleEntries.FindAll(x => x.Key == term).OrderBy(x => x.Priority).ToList();
            List<JmdictEntry> dictionaryEntries = new List<JmdictEntry>();

            if (matches is not null)
            {
                foreach (var item in matches)
                {
                    dictionaryEntries.Add((JmdictEntry)item.Value);
                }
                return dictionaryEntries;
            }

            //Revise wether it should return null, throw an exception or return an empty list.
            return [null];
        }

        public List<NameEntry> TranslateName(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Term cannot be null or empty");
            }

            SHA256 sha256 = SHA256.Create();

            //Here there could be a check on the size of term to send it to Analyzer straight away
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            int termIndex = BitConverter.ToUInt16(termHash, 0);
            List<ConversionEntry> possibleEntries = JapaneseDictionaryLoader.LoadPossibleJmnedictEntries(termIndex);

            //LINQ's are less efficient than foreach and especially parallel foreach, but at the size of this list
            //it doesn't matter. The conversions might matter, tough.
            List<ConversionEntry>? matches = possibleEntries.FindAll(x => x.Key == term).OrderBy(x => x.Priority).ToList();
            List<NameEntry> dictionaryEntries = new List<NameEntry>();

            if (matches is not null)
            {
                foreach (var item in matches)
                {
                    dictionaryEntries.Add((NameEntry)item.Value);
                }
                return dictionaryEntries;
            }

            //Revise wether it should return null, throw an exception or return an empty list.
            return [null];
        }

        public KanjiEntry TranslateKanji(char kanji)
        {
            int code = JapaneseAnalyzer.DetermineKanjiNumber(kanji);
            return JapaneseDictionaryLoader.LoadKanjiEntry(code);
        }

        public string TranslateKana(string kana)
        {           
            return kana.ToRomaji();
        }
    }
}
