using Mio.Translation;

namespace Mio.Translation.Japanese
{
    public class JapaneseLexeme
    {
        public string Surface { get; private set; }
        public string BaseForm { get; private set; }
        public GrammaticalCategory Category { get; private set; }

        public JapaneseLexeme(string surface, string[] features)
        {
            Surface = surface;
            Category = JapaneseAnalyzer.ParseToCategory(features[0]);
            switch (Category)
            {
                case GrammaticalCategory.Noun:
                    BaseForm = features[8];
                    break;
                case GrammaticalCategory.Pronoun:
                    BaseForm = features[8];
                    break;
                //Not sure this works 100% of the cases
                default:
                    BaseForm = features[7];
                    break;
            }
        }

        /// <summary>
        /// Only use for errored lexemes
        /// </summary>
        /// <param name="surface"></param>
        public JapaneseLexeme(string surface)
        {
            Surface = surface;
            Category = GrammaticalCategory.Error;
        }
    }
}
