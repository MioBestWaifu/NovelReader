using Mio.Reader.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class DataManagementService (ConfigurationsService configs)
    {

        public async Task<List<BookInteraction>> GetSavedInteractions()
        {
            try
            {
                List<BookInteraction> saved = JsonSerializer.Deserialize<List<BookInteraction>>(await
                    File.ReadAllTextAsync(Path.Combine(FileSystem.AppDataDirectory,"Library.json")), ConfigurationsService.jsonOptions);

                saved.RemoveAll(b => !File.Exists(b.Metadata.Path));

                return saved;
            } catch (Exception ex)
            {
                Debug.WriteLine($"Error getting saved interactions: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> SaveInteractions(List<BookInteraction> interactions)
        {
            try
            {
                if(interactions.Count == 0)
                {
                    return true;
                }
                await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, "Library.json"), JsonSerializer.Serialize(interactions, ConfigurationsService.jsonOptions));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving interactions: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DownloadUnidic()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan;

                    byte[] zipData = await client.GetByteArrayAsync("https://clrd.ninjal.ac.jp/unidic_archive/2302/unidic-cwj-202302.zip");

                    string appDataDir = FileSystem.AppDataDirectory;

                    string zipFilePath = Path.Combine(appDataDir, "unidic-cwj-202302.zip");

                    await File.WriteAllBytesAsync(zipFilePath, zipData);

                    string extractionDirPath = configs.PathToUnidic;

                    ZipFile.ExtractToDirectory(zipFilePath, extractionDirPath);

                    File.Delete(zipFilePath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error downloading and extracting ZIP file: {ex.Message}");
                return false;
            }
        }

        //The two method below are only useful for Android, but they do work elsewehere.

        /// <summary>
        /// Checks the permissions to read and writo to external storage, requests them if not already granted and returns the result.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestStoragePermissions()
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

        public async Task<string?> PickBook()
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
