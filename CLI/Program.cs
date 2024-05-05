using Maria.CLI.Input;
using Maria.Common.Communication;

namespace Maria.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(3000);
            CommandClient client = new CommandClient();
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
                if(modifiedCommand != null)
                    client.SendCommand(modifiedCommand);
            }
        }
    }
}
