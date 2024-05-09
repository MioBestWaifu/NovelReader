using Maria.Common.Communication.Commanding;
using Maria.Services.Translation.Japanese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Translation
{
    internal class TranslationCommandHandler : ICommandHandler
    {
        public async Task<string> HandleCommand(Command command)
        {
            switch (command.Submodule)
            {
                case "jp":
                    return await ProcessJapanese(command);
                default:
                    return $"{command.Submodule} translation not implemented";
            }
        }

        private static async Task<string> ProcessJapanese(Command command)
        {
            switch (command.Action)
            {
                case "start":
                    try
                    {
                        await Task.Run(JapaneseTranslator.Initialize);
                        return "Japanese translation started";
                    }
                    catch (Exception e)
                    {
                        return $"Error starting Japanese translation: {e.Message}";
                    }
                /*case "translate":
                    return await JapaneseTranslator.Translate(command.Parameters);*/
                default:
                    return $"{command.Action} translation not implemented as an action";
            }
        }
    }
}
