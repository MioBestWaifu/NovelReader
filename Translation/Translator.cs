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
        private Cache<List<ConversionEntry>> jmdictCache = new Cache<List<ConversionEntry>>(1000);
        private Cache<List<ConversionEntry>> namesdictCache = new Cache<List<ConversionEntry>>(1000);
        private Cache<KanjidicEntry> kanjidicCache = new Cache<KanjidicEntry>(1000);
        /// <summary>
        /// To translate into from the JMdict
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<JmdictEntry>> TranslateWord(string term)
        {
            List<ConversionEntry>? possibleMatches;
            int termIndex = GetHash(term);

            var cacheSearchResult = await jmdictCache.TryGetIndex(termIndex);
            if (cacheSearchResult.Item1)
            {
                possibleMatches = cacheSearchResult.Item2;
            }
            else
            {
                possibleMatches = await FindPossibleEntries(termIndex, DatabaseDictionary.JMdict);
            }

            if (possibleMatches is not null)
            {
                jmdictCache.Insert(termIndex, possibleMatches);
                possibleMatches = possibleMatches.FindAll(x => x.Key == term).OrderBy(x => x.Priority).ToList();
                List<JmdictEntry> dictionaryEntries = new List<JmdictEntry>();
                foreach (var item in possibleMatches)
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
        public async Task<NamedictEntry> TranslateName(string term)
        {
            List<ConversionEntry>? possibleMatches;
            int termIndex = GetHash(term);

            var cacheSearchResult = await namesdictCache.TryGetIndex(termIndex);
            if (cacheSearchResult.Item1)
            {
                possibleMatches = cacheSearchResult.Item2;
            }
            else
            {
                possibleMatches = await FindPossibleEntries(termIndex, DatabaseDictionary.JMnedict);
            }
            if (possibleMatches is not null)
            {
                namesdictCache.Insert(termIndex, possibleMatches);
                ConversionEntry? conversionEntry = possibleMatches.Find(x => x.Key == term);
                return conversionEntry != null ? (NamedictEntry)conversionEntry.Value : null;
            }

            return null;
        }

        private async Task<List<ConversionEntry>>? FindPossibleEntries(int termIndex, DatabaseDictionary dictionary)
        {
            List<ConversionEntry> possibleEntries;
            if (dictionary == DatabaseDictionary.JMdict)
            {
                return await DictionaryLoader.LoadPossibleJmdictEntries(termIndex);
            }
            else if (dictionary == DatabaseDictionary.JMnedict)
            {
                return await DictionaryLoader.LoadPossibleJmnedictEntries(termIndex);
            }
            else
            {
                throw new ArgumentException("Dictionary not supported");
            }
        }

        private int GetHash(string term){
            SHA256 sha256 = SHA256.Create();
            byte[] termHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(term));
            return BitConverter.ToUInt16(termHash, 0);
        }

        public async Task<KanjidicEntry> TranslateKanji(char kanji)
        {
            int code = Analyzer.DetermineKanjiNumber(kanji);
            var cacheSearchResult = await kanjidicCache.TryGetIndex(code);
            if (cacheSearchResult.Item1)
            {
                return cacheSearchResult.Item2;
            }
            var toReturn = await DictionaryLoader.LoadKanjiEntry(code);
            kanjidicCache.Insert(code, toReturn);
            return toReturn;
        }

        public string TranslateKana(string kana)
        {
            return kana.ToRomaji();
        }
    }
}
