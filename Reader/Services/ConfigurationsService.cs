using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Mio.Reader.Components;
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

        public ReadingManner ReadingManner { get; set; } = ReadingManner.Japanese;

        //Useless right now because to implement properly would require to adjust icons sizes too
        public int FontSize { get; set; } = 20;
        public DesignThemeModes Theme { get; set; } = DesignThemeModes.Dark;

        //Does nothing because this functionality is not implemented
        public bool TranslateSentences { get; set; } = true;
        public bool TranslateGeneral { get; set; } = true;
        public bool TranslateNames { get; set; } = true;
        public bool TranslateCharacters { get; set; } = true;
        //Does nothing because this functionality is not implemented
        public bool ShowFurigana { get; set; } = true;

        public ConfigurationsService()
        {

#if WINDOWS
            PathToUnidic = Path.Combine(AppContext.BaseDirectory, "Unidic");
#else
            PathToUnidic = Path.Combine(FileSystem.AppDataDirectory, "Unidic");
#endif
            Parser.configs = this;
        }

        public async void Save()
        {
            await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, "Configs.json"), JsonSerializer.Serialize(this, jsonOptions));
        }

        public ConfigurationsService Copy()
        {
            return MemberwiseClone() as ConfigurationsService;
        }

    }
}
