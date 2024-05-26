using Maria.CLI.Input;
using Maria.Common.Communication;

namespace Maria.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandClient client = new CommandClient();
            Validator.Initialize();
            Parser parser = new Parser();

            if (args.Length == 0)
            {
                Console.WriteLine("Awaiting commands, master");
                args = Console.ReadLine().Split(' ');
            }

            
            while (true)
            {
                //If this structure is kept, there should be a try-catch here
                var res = parser.Parse(args);

                foreach (var command in res)
                {
                    var validation = Validator.Validate(command, out var modifiedCommand, out var message);
                    Console.WriteLine($"\nOriginal command: {command}" +
                        $"\nResult: {validation}" +
                        $"\nModified command: {modifiedCommand}" +
                        $"\nMessage: {message}");
                    if (modifiedCommand != null)
                        client.SendCommand(modifiedCommand);
                }

                Console.WriteLine("Awaiting further commands, master");
                args = Console.ReadLine().Split(' ');
            }
        }
    }
}
