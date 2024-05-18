using Maria.Common.Communication;
using Maria.Services.Communication;
using Maria.Services.Experimentation;
using Maria.Services.Recordkeeping;
using Maria.Services.Recordkeeping.Records;
using MessagePack;

namespace Maria.Services
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
            commandServer.OnCommandReceived += (command) => Task.Run(() => interpreter.ProcessCommand(command));
            Writer.CreateInstance();
            Task.Run(() => Writer.Instance.FlushAll(600));
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
