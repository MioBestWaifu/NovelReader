using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    /*
     * This Service exists because the System image parsing is not realiable or good quality. However, ImageSharp is very
     * slow on Android, hence the need for plataform-specific implementations. The Android image parsing seems good so far. 
     */
    public abstract class ImageParsingService
    {
        public abstract Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry);

        public abstract Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry, int newWidth, int newHeight);
        public abstract Task<string> ParseImageBytesToBase64(byte[] bytes, string format);
        public abstract Task<string> ParseImageBytesToBase64(byte[] bytes, string format,int newWidth, int newHeight);
    }
}
