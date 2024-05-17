using Maria.Common.Communication;
using Maria.Services.Translation;
using Maria.Services.Translation.Japanese;
using Maria.Services.Translation.Japanese.Edrdg;
using MessagePack;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Maria.Services.Experimentation
{
    internal static class BenchmarkExperiments
    {
        private static string pathToData = @"Data\Japanese\";
        private static string pathToConvertedJmdict = pathToData + @"JMdict\";
        public static void AverageBinaryReading()
        {
            List<long> times = new List<long>();
            for (int i = 0; i < 200; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                byte[] data = File.ReadAllBytes($"{pathToConvertedJmdict}{i}.bin");
                var x = MessagePackSerializer.Deserialize<List<EdrdgEntry>>(data)!;
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Binary: {times.Average()} ms");
        }

        public static void AverageJsonReading()
        {
            List<long> times = new List<long>();
            for (int i = 0; i < 200; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string data = File.ReadAllText($"{pathToConvertedJmdict}{i}.json");
                var x = JsonSerializer.Deserialize<List<EdrdgEntry>>(data, CommandServer.jsonOptions)!;
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"JSON: {times.Average()} ms");
        }

        public static void AverageHashingTime()
        {
            ConcurrentDictionary<string, ConversionEntry> pairs = JapaneseDictionaryLoader.LoadConversionTable();
            List<string> keys = pairs.Keys.ToList();

            SHA256 sha256 = SHA256.Create();
            List<double> times = new List<double>();

            foreach (string key in keys)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                stopwatch.Stop();
                times.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine($"Hashing: {times.Average()} ms");
        }
    }
}
