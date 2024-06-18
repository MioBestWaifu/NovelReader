using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public abstract class ImageParsingService
    {
        public abstract Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry);

        public abstract Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry, int newWidth, int newHeight);
    }
}
