using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Formats;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

namespace Mio.Reader.Platforms.Windows
{
    public class WindowsImageParsingService : ImageParsingService
    {
        public override async Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry)
        {
            if (entry != null)
            {
                try
                {
                    using (var image = await ParseImage(entry))
                    {

                        using (var ms = new MemoryStream())
                        {
                            IImageEncoder encoder = entry.FullName.Split('.')[^1] switch
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

        public override async Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry, int newWidth, int newHeight)
        {
            if (entry != null)
            {
                try
                {
                    using (var image = await ParseImage(entry))
                    {
                        image.Mutate(x => x.Resize(newWidth, newHeight));

                        using (var ms = new MemoryStream())
                        {
                            IImageEncoder encoder = entry.FullName.Split('.')[^1] switch
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

        private async Task<Image> ParseImage(ZipArchiveEntry imageEntry)
        {
            using var stream = imageEntry.Open();
            return Image.Load(stream);
        }
    }
}
