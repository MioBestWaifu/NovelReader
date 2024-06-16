using Mio.Reader.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class ConfigurationsService
    {
        public static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
#if DEBUG
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
#endif
    };

        public string PathToUnidic { get; set; }
        //Multiple library folders to be implemented eventually, should be a list.
        public string PathToLibrary { get; set; }

        public string MainColor { get; set; } = "#770737";

        public ConfigurationsService()
        {
#if WINDOWS
            PathToUnidic = Path.Combine(AppContext.BaseDirectory, "Unidic");
#else
            PathToUnidic = Path.Combine(FileSystem.AppDataDirectory,"Unidic");
#endif
            EpubParser.Configs = this;
        }

        public async void Save()
        {
            await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, "Configs.json"), JsonSerializer.Serialize(this, jsonOptions));
        }

    }
}
