using Maria.Commons.Communication;
using Maria.Commons.Communication.Commanding;
using Maria.Translation.Japanese;

namespace Maria.Translation
{
    public class TranslationCommandHandler : ICommandHandler
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

        //How to pass errors? Should they go all the way back to CommandServer? Maybe in a tuple?
        //Also, response messages should be standardized.
        private static async Task<string> ProcessJapanese(Command command)
        {
            switch (command.Action)
            {
                case "start":
                    try
                    {
                        //await Task.Run(JapaneseTranslator.Initialize);
                        return "Japanese translation started";
                    }
                    catch (Exception e)
                    {
                        return $"Error starting Japanese translation: {e.Message}";
                    }
                case "stop":
                    //JapaneseTranslator.Dispose();
                    return "Japanese translation stopped";
                case "translate":
                    //return await Task.Run(() => JapaneseTranslator.Instance?.Translate(command));
                case "create":
                    //JapaneseDictionaryCreator.CreateDictionary();
                    return "Sucess";
                default:
                    return $"{command.Action} translation not implemented as an action";
            }
        }
    }
}
