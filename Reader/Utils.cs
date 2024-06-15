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

namespace Mio.Reader
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

        public static async Task<string> GetCoverBase64(string pathToEpub, string coverRelativePath)
        {
            ZipArchive archive = ZipFile.OpenRead(pathToEpub);
            Dictionary<string, ZipArchiveEntry> namedEntries = new Dictionary<string, ZipArchiveEntry>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                namedEntries[entry.FullName] = entry;
            }

            string containerXml = await new StreamReader(namedEntries["META-INF/container.xml"].Open()).ReadToEndAsync();

            string standardOpfPath = await EpubMetadataResolver.ResolveStandardsFile(containerXml);

            ZipArchiveEntry coverEntry = GetRelativeEntry(namedEntries[standardOpfPath], coverRelativePath);
            //Could use some compression
            return await GetCoverBase64(coverEntry, true);
        }

        public static async Task<string> GetCoverBase64(ZipArchiveEntry coverEntry, bool downscale)
        {
            if (coverEntry != null)
            {
                try
                {
                    using (var stream = coverEntry.Open())
                    {
                        using (var image = SixLabors.ImageSharp.Image.Load(stream))
                        {
                            if (downscale)
                            {
                                // Set the new size here
                                int newWidth = 220;
                                int newHeight = 330;

                                image.Mutate(x => x.Resize(newWidth, newHeight));
                            }

                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                return Convert.ToBase64String(ms.ToArray());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return "";
        }


        public static async Task<bool> RequestStoragePermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();

                if (status == PermissionStatus.Granted)
                {
                    status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    }
                }
            }

            return status == PermissionStatus.Granted;
        }


    }
}
