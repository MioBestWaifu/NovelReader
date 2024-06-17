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
            return await ParseImageEntryToBase64(coverEntry, 440, 660);
        }

        public static async Task<string> ParseImageEntryToBase64(ZipArchiveEntry coverEntry, int newWidth, int newHeight)
        {
            if (coverEntry != null)
            {
                try
                {
                        using (var image = await ParseImage(coverEntry))
                        {
                            image.Mutate(x => x.Resize(newWidth, newHeight));

                            using (var ms = new MemoryStream())
                            {
                            IImageEncoder encoder = coverEntry.FullName.Split('.')[^1] switch
                            {
                                "jpg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                                "jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                                "png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                                "gif" => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                                _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            };
                            image.Save(ms,encoder);
                                return Convert.ToBase64String(ms.ToArray());
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

        public static async Task<string> ParseImageEntryToBase64(ZipArchiveEntry imageEntry)
        {
            if (imageEntry != null)
            {
                try
                {
                    using (var image = await ParseImage(imageEntry))
                    {

                        using (var ms = new MemoryStream())
                        {
                            IImageEncoder encoder = imageEntry.FullName.Split('.')[^1] switch
                            {
                                "jpg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                                "jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                                "png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                                "gif" => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                                _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            };
                            image.Save(ms, encoder);
                            return Convert.ToBase64String(ms.ToArray());
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

        public static async Task<Image> ParseImage(ZipArchiveEntry imageEntry)
        {
            using var stream = imageEntry.Open();
            return SixLabors.ImageSharp.Image.Load(stream);
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

        public static async Task<string> PickFileAndroid()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/epub+zip" } }, // MIME type for EPUB files
                    // Add other platforms if necessary
                });

                var options = new PickOptions
                {
                    PickerTitle = "Please select an EPUB file",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    return result.FullPath;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if any
                Debug.WriteLine($"Error picking file: {ex.Message}");
            }

            return null; 
        }


    }
}
