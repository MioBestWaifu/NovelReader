using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.LifecycleEvents;

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using CommunityToolkit.Maui.Core;

#if WINDOWS
After:
using CommunityToolkit.Maui.Core;
using Mio.Reader.Services;


#if WINDOWS
*/
using CommunityToolkit.Maui.Core;
using Mio.Reader.Services;

#if WINDOWS
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

            builder.Services.AddSingleton<ConfigurationsService>();
            builder.Services.AddSingleton<DataManagementService>();
            builder.Services.AddSingleton<LibraryService>();


            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }


    }

}
