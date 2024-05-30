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
            //new JapaneseTranslator("D:\\Programs\\Maria-chan\\Services\\Translation\\JMDict\\");
            JapaneseAnalyzer analyzer = new JapaneseAnalyzer("D:\\Programs\\Data\\Unidic");
            analyzer.Analyze("高い");
            analyzer.Analyze("速く");
            analyzer.Analyze("そして");
            analyzer.Analyze("が");
            analyzer.Analyze("です");
            analyzer.Analyze("ああ");
            analyzer.Analyze("ええと");
            analyzer.Analyze("こんにちは");
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
