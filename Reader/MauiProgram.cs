using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.LifecycleEvents;

using CommunityToolkit.Maui.Core;
using Mio.Reader.Services;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Text.Json;

#if ANDROID
using Mio.Reader.Platforms.Android;
#endif

#if WINDOWS
using Mio.Reader.Platforms.Windows;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using WinRT.Interop;
using Windows.Graphics;
#endif

namespace Mio.Reader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseMauiCommunityToolkitCore();


            builder.ConfigureLifecycleEvents(lifecycle =>
            {

#if WINDOWS
                lifecycle.AddWindows(windowBuilder =>
                {

                    windowBuilder.OnWindowCreated(window =>
                    {
                        AppWindow winuiAppWindow = window.AppWindow;
                        window.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                        OverlappedPresenter p = (OverlappedPresenter)window.AppWindow.Presenter;
                        p.Maximize();
                    });
                });
#endif
            });
            ConfigurationsService configurationsService;
            try
            {
                var readTask = File.ReadAllTextAsync(Path.Combine(FileSystem.AppDataDirectory,"Configs.json"));
                readTask.Wait();
                configurationsService = JsonSerializer.Deserialize<ConfigurationsService>(readTask.Result,ConfigurationsService.jsonOptions);
            } catch (Exception)
            {
                configurationsService = new ConfigurationsService();
            }

            builder.Services.AddSingleton(configurationsService!);
            builder.Services.AddSingleton<DataManagementService>();
            builder.Services.AddSingleton<LibraryService>();
#if WINDOWS
            builder.Services.AddSingleton<ImageParsingService, WindowsImageParsingService>();
#elif ANDROID
            builder.Services.AddSingleton<ImageParsingService, AndroidImageParsingService>();
#endif

            builder.Services.AddFluentUIComponents();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }


    }

}
