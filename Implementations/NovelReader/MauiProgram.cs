using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Maria.NovelReader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            /*builder.Services.AddSingleton<IFileProvider>(sp =>
            {
                var provider = new CompositeFileProvider(
                    new PhysicalFileProvider(Path.Combine(FileSystem.AppDataDirectory, "wwwroot")),
                    new ManifestEmbeddedFileProvider(typeof(YourRazorClassLibrary.SomeTypeInLibrary).Assembly, "wwwroot")
                );
                return provider;
            });*/

            return builder.Build();
        }
    }
}
