using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.LifecycleEvents;
using CommunityToolkit.Maui.Core;

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
                .UseMauiApp<App>();

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


            builder.Services.AddMauiBlazorWebView();

            builder.UseMauiApp<App>().UseMauiCommunityToolkitCore();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }


    }

}
