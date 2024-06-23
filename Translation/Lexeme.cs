namespace Mio.Translation
{
    public class Lexeme
    {
        public string Surface { get; private set; }
        public string BaseForm { get; private set; }
        public GrammaticalCategory Category { get; private set; }

        public Lexeme(string surface, string[] features)
        {
            Surface = surface;
            Category = Analyzer.ParseToCategory(features[0]);
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
        public Lexeme(string surface)
        {
            Surface = surface;
            Category = GrammaticalCategory.Error;
        }
    }
}
