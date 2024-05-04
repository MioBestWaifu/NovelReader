using Maria.CLI.Interpretation;

namespace Maria.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            parser.Parse(args);
        }
    }
}
