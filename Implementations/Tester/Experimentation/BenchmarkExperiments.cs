using Maria.Commons.Communication;
using Maria.Commons.Recordkeeping;
using Maria.Translation.Japanese.Edrdg;
using MessagePack;
using System.Diagnostics;
using System.Text.Json;

namespace Maria.Tester.Experimentation
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
                var x = Serializer.DeserializeJson<List<EdrdgEntry>>(data)!;
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"JSON: {times.Average()} ms");
        }
    }
}
