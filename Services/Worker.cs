using Maria.Common.Communication;
using Maria.Services.Communication;
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

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            commandServer = new CommandServer();
            interpreter = new Interpreter();
            commandServer.OnCommandReceived += (command) => Task.Run(() => interpreter.ProcessCommand(command));
            Writer.CreateInstance();
            Task.Run(() => Writer.Instance.FlushAll(60));
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

        //Why is this here? Because i dont want to put MessagePack in Common. Maybe should.
        private static void TestDeserialization()
        {
            byte[] data = File.ReadAllBytes(@"D:\Programs\maria-chan\Tests\browser\2024\5\17\16-50-44.bin");
            var records = MessagePackSerializer.Deserialize<List<TrackingRecord>>(data);
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }
    }
}
