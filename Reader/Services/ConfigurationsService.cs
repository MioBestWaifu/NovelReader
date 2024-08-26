using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Mio.Reader.Components;
using Mio.Reader.Parsing;
using Mio.Reader.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class ConfigurationsService
    {
        public static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new MetadataConverter() },
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
        }

        public async void Save()
        {
            await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, "Configs.json"), JsonSerializer.Serialize(this, jsonOptions));
        }

        public ConfigurationsService Copy()
        {
            return MemberwiseClone() as ConfigurationsService;
        }

        private class MetadataConverter : JsonConverter<BookMetadata>
        {
            public override BookMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("Version", out _))
                    {
                        return JsonSerializer.Deserialize<EpubMetadata>(root.GetRawText(), options);
                    } 

                    return JsonSerializer.Deserialize<PdfMetadata>(root.GetRawText(), options);
                }
            }

            public override void Write(Utf8JsonWriter writer, BookMetadata value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
            }
        }

    }
}
