using System.Diagnostics;

namespace Mio.Reader
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            var x = FileSystem.Current.AppDataDirectory;
            Debug.WriteLine(x);
        }
    }


}
