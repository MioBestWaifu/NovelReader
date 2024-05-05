using Maria.Services.Communication.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.CLI.Interpretation
{
    internal class Validator
    {
        private static readonly List<string> validRoots = ["tracking","tell"];

        private static Dictionary<string, CommandDefinition> definitions;

        public static void Initialize()
        {

        }

        /// <summary>
        /// Validates the command, editing it if necessary. Always use the modified command for execution.
        /// </summary>
        /// <param name="originalCommand"></param>
        /// <param name="modifiedCommand">A version of the command modified for proper execution. Changes may be due to partial non-executability of the command or due to internal necessities in fully valid commands. As such, it is this modified version that should be sent for executtion, irrespective os validation results.</param>
        /// <param name="message">Explains what, if anything, is wrong with the command</param>
        /// <returns>If the command is completely invalid, such as if it misses an action or required suffix, returns<see cref="ValidationResult.InvalidNonExecutable"/>. If it is understandable but some parts are incorrect, such as options, returns <see cref="ValidationResult.InvalidExecutable"/> but modifies the command as needed. </returns>
        public static ValidationResult Validate(Command originalCommand, out Command modifiedCommand, out string message)
        {
            modifiedCommand = null;
            message = string.Empty;
            bool invalid = false;

            if (string.IsNullOrEmpty(originalCommand.Action))
            {
                message = "The command is missing an action.";
                return ValidationResult.InvalidNonExecutable;
            } else if (!IsValidAction(originalCommand.Action))
            {
                message = $"Action \"{originalCommand.Action}\" is not a valid. Valid actions are: {string.Join(", ", definitions.Keys)}";
                return ValidationResult.InvalidNonExecutable;
            }

            CommandDefinition definition = definitions[originalCommand.Action];
            modifiedCommand = new Command(originalCommand.Action);

            if (definition.Suffix.Required && string.IsNullOrEmpty(originalCommand.Suffix))
            {
                message = $"The command is missing a required suffix.";
                return ValidationResult.InvalidNonExecutable;
            } else if ( !definition.Suffix.Values.Contains(originalCommand.Suffix))
            {
                if (definition.Suffix.Required)
                {
                    message = $"The suffix \"{originalCommand.Suffix}\" is not valid for action \"{originalCommand.Action}\". Valid suffixes are: {string.Join(", ", definition.Suffix.Values)}";
                    return ValidationResult.InvalidNonExecutable;
                } else
                {
                    message = $"The suffix \"{originalCommand.Suffix}\" is not valid for action \"{originalCommand.Action}\". Valid suffixes are: {string.Join(", ", definition.Suffix.Values)}";
                    invalid = true;
                }
            }

            modifiedCommand.Suffix = originalCommand.Suffix;

            foreach (var prefix in originalCommand.Prefixes)
            {
                PrefixDefinition prefixDefinition;
                bool hasPrefix = definition.Prefixes.TryGetValue(prefix, out prefixDefinition);

                //This has to take into account time-based prefixes. Maybe they should be separated. 
                if (!hasPrefix)
                {
                    message += $"\nPrefix \"{prefix}\" is not valid for action \"{originalCommand.Action}\".";
                    invalid = true;
                } else if (prefixDefinition.Mutex.Any(p => originalCommand.Prefixes.Contains(p)))
                {
                    message += $"\nPrefix \"{prefix}\" is mutually exclusive with the following prefixes: {string.Join(", ", prefixDefinition.Mutex)}.";
                    invalid = true;
                } else
                {
                    modifiedCommand.Prefixes.Add(prefix);
                }
            }

            foreach(var pair in originalCommand.Options)
            {
                if (!definition.Options.ContainsKey(pair.Key))
                {
                    message += $"\nOption \"{pair.Key}\" is not valid for action \"{originalCommand.Action}\".";
                    invalid = true;
                } else if (!IsValidOptionValue(definition.Options[pair.Key],pair.Value,message))
                {
                    invalid = true;
                } else
                {
                    modifiedCommand.Options.Add(pair.Key, pair.Value);
                }
            }

            return invalid ? ValidationResult.InvalidExecutable : ValidationResult.Valid;

        }

        public static bool IsValidAction(string word)
        {
           return definitions.ContainsKey(word);
        }

        private static bool IsValidOptionValue(OptionDefinition definition, string value, string message)
        {
            return definition.Values.Contains(value);
        }
    }
}
