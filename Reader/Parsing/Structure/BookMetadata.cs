using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mio.Reader.Parsing.Structure
{
    public class BookMetadata
    {

        public string Title { get; set; }
        public string Author { get; set; }
        public string Path { get; set; }
        [JsonIgnore]
        public string CoverBase64 { get; set; }

        public BookMetadata(string title, string author, string path, string coverBase64)
        {
            Title = title;
            Author = author;
            Path = path;
            CoverBase64 = coverBase64;
        }
    }
}
