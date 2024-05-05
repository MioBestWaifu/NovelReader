using Maria.CLI.Input;

namespace Maria.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Validator.Initialize();
            Parser parser = new Parser();
            var res = parser.Parse(args);

            foreach (var command in res)
            {
                var validation = Validator.Validate(command, out var modifiedCommand, out var message);
                Console.WriteLine($"\nOriginal command: {command}" +
                    $"\nResult: {validation}" +
                    $"\nModified command: {modifiedCommand}" +
                    $"\nMessage: {message}");
            }
        }
    }
}
