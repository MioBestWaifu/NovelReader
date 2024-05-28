using Maria.Commons.Communication;
using Maria.Translation;
using Maria.Translation.Japanese;

namespace Maria.Tester
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CommandServer commandServer;
        private readonly Interpreter interpreter;

        public Worker(ILogger<Worker> logger, IHostEnvironment environment)
        {
            _logger = logger;
            Constants.Initialize(environment.IsDevelopment());
            commandServer = new CommandServer();
            interpreter = new Interpreter();
            JapaneseTranslator.PathToDictionary = Constants.Paths.ToConvertedDictionary;
            JapaneseTranslator.PathToUnidic = Constants.Paths.ToUnidic;
            JapaneseTranslator.Initialize();
            /*interpreter.RegisterHandler(new TranslationCommandHandler(), "translation");
            commandServer.OnCommandReceived += (command) => Task.Run(() => interpreter.ProcessCommand(command));*/
            Console.WriteLine(JapaneseTranslator.Instance.Translate(new Commons.Communication.Commanding.Command()
            {
                Options =
                {
                    {"term","踏ん反る" }
                }
            }));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
            }
        }
    }
}
