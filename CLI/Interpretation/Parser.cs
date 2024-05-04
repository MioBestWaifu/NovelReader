using Maria.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.CLI.Interpretation
{
    internal class Parser
    {
        public List<Command> Parse(string[] args)
        {
            var result = new List<Command>();
            Dictionary<string, string> modifiers = new Dictionary<string, string>();
            List<string> prefixes = new List<string>();
            Command currentCommand = null;
            bool isInModifiers = false; int modifierCount = 0;
            for (int i = 0; i < args.Length; i ++)
            {
                var arg = args[i];
                

                if (Validator.IsValidRoot(arg))
                {
                    currentCommand = new Command(arg.TrimEnd(',', ';'));
                    currentCommand.Prefixes = prefixes;
                    currentCommand.Options = new List<string>();
                } else if (!isInModifiers)
                {
                    if (currentCommand != null)
                    {
                        currentCommand.Options.Add(arg.TrimEnd(',',';'));
                    }
                    else
                    {
                        prefixes.Add(arg.TrimEnd(',', ';'));
                    }
                } else
                {
                    if (modifierCount == 1)
                    {
                        modifiers.Add(args[i - 1].TrimEnd(',', ';'), arg.TrimEnd(',', ';'));
                        modifierCount = 0;
                    } else
                    {
                        modifierCount++;
                    }
                }

                if (!isInModifiers && arg.EndsWith(','))
                {
                    isInModifiers = true;
                } else if (arg.EndsWith(';'))
                {
                    currentCommand.Modifiers = modifiers;
                    result.Add(currentCommand);
                    currentCommand = null;
                    prefixes = new List<string>();
                    modifiers = new Dictionary<string, string>();
                    modifierCount = 0;
                    isInModifiers = false;
                    continue;
                }
            }

            if(currentCommand != null)
            {
                currentCommand.Modifiers = modifiers;
                result.Add(currentCommand);
            }

            foreach (var command in result)
            {
                Console.WriteLine($"Root: {command.Root}");
                Console.WriteLine($"Prefixes: {string.Join(", ", command.Prefixes)}");
                Console.WriteLine($"Options: {string.Join(", ", command.Options)}");
                Console.WriteLine($"Modifiers: {string.Join(", ", command.Modifiers)}");
            }

            return result;
        }
    }
}
