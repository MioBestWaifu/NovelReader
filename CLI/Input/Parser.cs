using Maria.CLI.Exceptions;
using Maria.Services.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.CLI.Input
{
    internal class Parser
    {
        public List<Command> Parse(string[] args)
        {
            var result = new List<Command>();
            Dictionary<string, string> options = new Dictionary<string, string>();
            List<string> prefixes = new List<string>();
            Command currentCommand = new Command();
            bool isInOptions = false, reInit = false; int optionCount = 0;
            for (int i = 0; i < args.Length; i ++)
            {
                if (reInit)
                {
                    currentCommand = new Command();
                    prefixes = new List<string>();
                    options = new Dictionary<string, string>();
                    optionCount = 0;
                    isInOptions = false;
                    reInit = false;
                }

                var arg = args[i];
                

                if (Validator.IsValidAction(arg))
                {
                    currentCommand.Action = arg.TrimEnd(',', ';').ToLower();
                    currentCommand.Prefixes = prefixes;
                } else if (!isInOptions)
                {
                    if (!string.IsNullOrEmpty(currentCommand.Action))
                    {
                        if (!string.IsNullOrEmpty(currentCommand.Suffix))
                            throw new MultipleSuffixesException();
                        currentCommand.Suffix = arg.TrimEnd(',',';');
                    }
                    else
                    {
                        prefixes.Add(arg.TrimEnd(',', ';').ToLower().ToLower());
                    }
                } else
                {
                    if (optionCount == 1)
                    {
                        options.Add(args[i - 1].TrimEnd(',', ';').ToLower(), arg.TrimEnd(',', ';').ToLower());
                        optionCount = 0;
                    } else
                    {
                        optionCount++;
                    }
                }

                if (!isInOptions && arg.EndsWith(','))
                {
                    isInOptions = true;
                } else if (arg.EndsWith(';') && i < args.Length - 1)
                {
                    //Remember to deal with cases where there is no valid action
                    currentCommand.Options = options;
                    result.Add(currentCommand);
                    reInit = true;
                    continue;
                }
            }
            currentCommand.Prefixes = prefixes;
            currentCommand.Options = options;
            result.Add(currentCommand);

            foreach (var command in result)
            {
                Console.WriteLine($"Prefixes: {string.Join(", ", command.Prefixes)}");
                Console.WriteLine($"Action: {command.Action}");
                Console.WriteLine($"Suffix: {string.Join(", ", command.Suffix)}");
                Console.WriteLine($"Options: {string.Join(", ", command.Options)}");
            }

            return result;
        }
    }
}
