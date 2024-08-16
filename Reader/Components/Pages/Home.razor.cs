using CommunityToolkit.Maui.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Mio.Reader.Parsing.Loading;
using Mio.Reader.Parsing.Structure;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Components.Pages
{
    public partial class Home
    {
        ShortcutResponder ShortcutResponder { get; } = new ShortcutResponder();

        [Inject]
        IJSRuntime JS { get; set; }
        [Inject]
        public NavigationManager Navigator { get { 
                return navigator;
            } set { 
                navigator = value;
                ShortcutResponder.Navigator = navigator;
            } 
        }

        private NavigationManager navigator;
        [Inject]
        ConfigurationsService Configurations { get; set; }
        [Inject]
        DataManagementService DataManager { get; set; }
        [Inject]
        LibraryService Lib { get; set; }

        DevicePlatform plataform = DeviceInfo.Current.Platform;
        bool UnidicDetermined { get; set; } = false;

        protected override Task OnInitializedAsync()
        {
            JS.InvokeVoidAsync("changeDirectionToLTR");
            if (plataform == DevicePlatform.Android)
            {
                UnidicDetermined = Directory.Exists(Configurations.PathToUnidic);
                InvokeAsync(() => { StateHasChanged(); });
            }
            return base.OnInitializedAsync();
        }

        private async void PickLibraryFolder()
        {

            if (plataform == DevicePlatform.Android)
            {
                if (!await DataManager.RequestStoragePermissions())
                    return;
                /*
                * This is a workaraound and should ideally not exist. The problem is that Android refuses to list the files in the directory.
                * I have permissions, have used differente permissions and API's (including Intent and Java), but the system just refuses to return all files in the library fodler.
                * The odd things are that the exact same call on the exact same folder works on another project, and that here one file (jpg) is listed.
                * Until i can figure out what the hell is going on, android only allows to pick specific files, not a library.
                */
                string? filePath = await DataManager.PickBook();
                if (string.IsNullOrEmpty(filePath))
                    return;
                Task task = Task.Run(async () => Lib.Books.Add(new BookInteraction(await BookLoader.LoadMetadata(filePath))));
                await task;
                Navigator.NavigateTo("/reader?bookIndex=0");
                return;
            }

            FolderPickerResult result = await FolderPicker.Default.PickAsync();
            if (!result.IsSuccessful)
                return;

            Configurations.PathToLibrary = result.Folder.Path;
            Configurations.Save();
            Debug.WriteLine(Configurations.PathToLibrary);
            InvokeAsync(() => StateHasChanged());
        }

        private async void DownloadUnidic()
        {
            UnidicDetermined = await DataManager.DownloadUnidic();

            InvokeAsync(() => StateHasChanged());
        }
    }
}
