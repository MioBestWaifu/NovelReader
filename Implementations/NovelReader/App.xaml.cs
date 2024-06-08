using System.Diagnostics;

namespace Maria.NovelReader
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            /*AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Debug.WriteLine(e.ExceptionObject);
                Debug.WriteLine(s);
            };*/
        }
    }


}
