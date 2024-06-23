using Mio.Reader.Parsing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Formats;

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using Mio.Translation.Japanese.Edrdg;
After:
using Mio.Translation.Japanese.Edrdg;
using Mio.Translation.Edrdg;
*/

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using Mio.Translation.Edrdg;
After:
using Mio.Translation.Edrdg;
using Mio.Translation.Elements;
*/

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using Mio.Translation.Elements;
After:
using Mio.Translation.Elements;
using Mio.Translation.Properties;
*/
using Mio.Translation.Elements;
using Mio.Translation.Properties;




#if ANDROID
using Java.Util;
#endif

namespace Mio.Reader
{
    internal static class Utils
    {
        public static ZipArchiveEntry GetRelativeEntry(ZipArchiveEntry currentLocation, string relativePath)
        {
            ZipArchive archive = currentLocation.Archive;
            string directory;
            int lastSlash = currentLocation.FullName.LastIndexOf('/');
            if (relativePath.StartsWith('/'))
            {
                directory = relativePath.Substring(1);
            } 
            else if (lastSlash == -1)
            {
                directory = relativePath;
            }
            else
            {
                // Get the directory of the current location
                directory = currentLocation.FullName.Substring(0, lastSlash);

                // Split the relative path into parts
                string[] parts = relativePath.Split('/');

                foreach (string part in parts)
                {
                    if (part == "..")
                    {
                        // Go up one level
                        lastSlash = directory.LastIndexOf('/');
                        if(lastSlash == -1)
                        {
                            directory = "";
                        }
                        else
                        {
                            directory = directory.Substring(0, lastSlash);
                        }
                    }
                    else
                    {
                        // Go down to the next level
                        directory += "/" + part;
                    }
                }
            }

            if (directory.StartsWith('/'))
            {
                directory = directory.Substring(1);
            }
            // Get the entry with the full path
            ZipArchiveEntry entry = archive.GetEntry(directory);

            return entry;
        }

        public static async Task<ZipArchiveEntry> GetCoverEntry(string pathToEpub, string coverRelativePath)
        {
            ZipArchive archive = ZipFile.OpenRead(pathToEpub);
            Dictionary<string, ZipArchiveEntry> namedEntries = new Dictionary<string, ZipArchiveEntry>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                namedEntries[entry.FullName] = entry;
            }

            string containerXml = await new StreamReader(namedEntries["META-INF/container.xml"].Open()).ReadToEndAsync();

            string standardOpfPath = await EpubMetadataResolver.ResolveStandardsFile(containerXml);

            return GetRelativeEntry(namedEntries[standardOpfPath], coverRelativePath);
        }

        public static string JoinKanjis(List<KanjiElement>? kanjis)
        {
            return kanjis != null ? string.Join(", ", kanjis.Select(k => k.Kanji)) : string.Empty;
        }

        public static string JoinReadings(List<ReadingElement> readings)
        {
            return readings != null ? string.Join(", ", readings.Select(r => r.Reading)) : string.Empty;
        }

        public static string BuildKanjiObservations(KanjiElement element)
        {
            IOrderedEnumerable<KanjiProperty> properties = element.Properties.Order();
            if(properties.Count() == 0)
            {
                return string.Empty;
            }
            return $"({string.Join(", ", properties.Select(p => PropertyConverter.KanjiPropertyToShortString(p).ToLower()))})";
        }

        public static string BuildReadingObservations(ReadingElement element)
        {
            var properties = element.Properties.Order();
            if (properties.Count() == 0)
            {
                return string.Empty;
            }
            return $"({string.Join(", ", properties.Select(p => InsertWhiteSpaceOnUpperCase(p.ToString()).ToLower()))})";
        }

        public static string BuildSenseObervations(SenseElement element)
        {
            var fields = element.Fields.Order();
            var misc = element.MiscProperties.Order();
            var allStrings = fields.Select(f => f.ToString()).ToList();
            allStrings.AddRange(misc.Select(f => f.ToString()));
            if (allStrings.Count == 0)
            {
                return string.Empty;
            }
            return $"({string.Join(", ", allStrings.Select(p => InsertWhiteSpaceOnUpperCase(p).ToLower()))})";
        }

        public static string BuildNameTypeObservations(List<NameType> types)
        {
            if (types.Count() == 0)
            {
                return string.Empty;
            }
            return $"({string.Join(", ", types.Order().Select(p => InsertWhiteSpaceOnUpperCase(p.ToString()).ToLower()))})";
        }

        public static string InsertWhiteSpaceOnUpperCase(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            StringBuilder sb = new StringBuilder(word.Length * 2);
            sb.Append(word[0]); // Add the first character as is

            for (int i = 1; i < word.Length; i++)
            {
                if (char.IsUpper(word[i]))
                {
                    sb.Append(' ');
                }
                sb.Append(word[i]);
            }

            return sb.ToString();
        }

        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
