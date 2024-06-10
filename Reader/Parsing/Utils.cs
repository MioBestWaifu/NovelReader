using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing
{
    internal static class Utils
    {
        public static ZipArchiveEntry GetRelativeEntry(ZipArchiveEntry currentLocation, string relativePath)
        {
            ZipArchive archive = currentLocation.Archive;
            string directory;
            if (relativePath.StartsWith('/'))
            {
                directory = relativePath.Substring(1);
            }
            else
            {
                // Get the directory of the current location
                directory = currentLocation.FullName.Substring(0, currentLocation.FullName.LastIndexOf('/'));

                // Split the relative path into parts
                string[] parts = relativePath.Split('/');

                foreach (string part in parts)
                {
                    if (part == "..")
                    {
                        // Go up one level
                        directory = directory.Substring(0, directory.LastIndexOf('/'));
                    }
                    else
                    {
                        // Go down to the next level
                        directory += "/" + part;
                    }
                }
            }

            // Get the entry with the full path
            ZipArchiveEntry entry = archive.GetEntry(directory);

            return entry;
        }


    }
}
