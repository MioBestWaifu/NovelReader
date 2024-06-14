using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Managers
{
    internal class DataManager
    {
        public static async Task<bool> DownloadUnidic()
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

                    string extractionDirPath = Configurations.Current.PathToUnidic;

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
    }
}
