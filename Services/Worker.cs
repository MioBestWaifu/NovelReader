using Maria.Common.Communication;
using Maria.Services.Communication;
using Maria.Services.Recordkeeping;

namespace Maria.Services
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CommandServer commandServer;
        private readonly Interpreter interpreter;

        public Worker(ILogger<Worker> logger, Writer writer)
        {
            _logger = logger;
            commandServer = new CommandServer();
            interpreter = new Interpreter();
            commandServer.OnCommandReceived += (command) => Task.Run(() => interpreter.ProcessCommand(command));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
