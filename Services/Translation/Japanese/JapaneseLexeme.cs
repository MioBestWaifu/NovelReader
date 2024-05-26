
namespace Maria.Translation.Japanese
{
    internal class JapaneseLexeme
    {
        public string Surface { get; private set; }
        public string BaseForm { get; private set; }
        public GrammaticalCategory Category { get; private set; }

        public JapaneseLexeme(string surface, string category, string baseForm)
        {
            Surface = surface;
            BaseForm = baseForm;
            Category = JapaneseAnalyzer.ParseToCategory(category);
        }
    }
}
