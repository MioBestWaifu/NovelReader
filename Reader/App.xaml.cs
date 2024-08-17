using Mio.Reader.Parsing;
using Mio.Reader.Parsing.Loading;
using Mio.Reader.Services;
using System.Diagnostics;

namespace Mio.Reader
{
    public partial class App : Application
    {
        LibraryService library;
        public App(LibraryService library, ImageParsingService imageParsingService)
        {
            EpubMetadataResolver.Initialize(imageParsingService);
            Parser.Initialize(imageParsingService);
            InitializeComponent();
            this.library = library;

            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Stopped += (s, e) =>
            {
                library.SaveAll();
            };

            window.Destroying += (s, e) =>
            {
                //Not sure if the OS will always wait for this to finish. I guess Windows will and Android depends on the device,
                //in no small part because file writing on desktop should be faster
                library.SaveAll();
            };

            return window;
        }
    }


}
