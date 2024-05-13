using Maria.Services.Translation;
using Maria.Services.Translation.Japanese;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Experimentation
{
    internal class ShaHashExperiments
    {
        public static void JpKeysToMillionRange()
        {
            ConcurrentDictionary<string, ConversionEntry> pairs = JapaneseDictionaryLoader.LoadConversionTable();
            List<string> keys = pairs.Keys.ToList();

            SHA256 sha256 = SHA256.Create();
            List<BigInteger> hashNumbers = new List<BigInteger>();

            foreach (string key in keys)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                BigInteger hashNumber = new BigInteger(hash);
                hashNumber = (hashNumber % 1000000) + 1; // to bring it in the range of 1 to 1000000
                hashNumbers.Add(hashNumber);
            }

            int collisionCount = hashNumbers.Count - hashNumbers.Distinct().Count();
            Console.WriteLine($"Number of collisions: {collisionCount}");

        }

        public static void JpKeysToFileIndex()
        {
            ConcurrentDictionary<string, ConversionEntry> pairs = JapaneseDictionaryLoader.LoadConversionTable();
            List<string> keys = pairs.Keys.ToList();

            SHA256 sha256 = SHA256.Create();
            int[] counts = new int[256];

            foreach (string key in keys)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                int number = hash[0]; // take the first 8 bits
                counts[number]++;
            }

            for (int i = 0; i < counts.Length; i++)
            {
                Console.WriteLine($"Number {i}: {counts[i]} times");
            }
        }

        public static void JpKeysToIndexAnd11BitOffset()
        {
            ConcurrentDictionary<string, ConversionEntry> pairs = JapaneseDictionaryLoader.LoadConversionTable();
            List<string> keys = pairs.Keys.ToList();

            SHA256 sha256 = SHA256.Create();
            Dictionary<int, Dictionary<int, int>> indexOffsetCounts = new Dictionary<int, Dictionary<int, int>>();
            int totalCollisions = 0;

            foreach (string key in keys)
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                int index = hash[0]; // take the first 8 bits
                int offset = (hash[1] << 3 | hash[2] >> 5) & 0x7FF; // take the next 11 bits

                if (!indexOffsetCounts.ContainsKey(index))
                {
                    indexOffsetCounts[index] = new Dictionary<int, int>();
                }

                if (!indexOffsetCounts[index].ContainsKey(offset))
                {
                    indexOffsetCounts[index][offset] = 0;
                }
                else
                {
                    totalCollisions++;
                }

                indexOffsetCounts[index][offset]++;
            }

            foreach (var index in indexOffsetCounts.Keys)
            {
                foreach (var offset in indexOffsetCounts[index].Keys)
                {
                    Console.WriteLine($"Index {index}, Offset {offset}: {indexOffsetCounts[index][offset]} times");
                }
            }

            Console.WriteLine($"Total collisions: {totalCollisions}");
        }
    }
}
