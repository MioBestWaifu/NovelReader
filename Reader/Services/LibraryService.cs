using Mio.Reader.Parsing;
using Mio.Reader.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class LibraryService (ConfigurationsService configs,DataManagementService dataManager)
    {
        //Maybe should make this another class containing both the metadata and the user's interactions.
        public List<EpubMetadata> Books { get; private set; } = [];
        public event EventHandler BookAdded;

        public async void Initialize()
        {
            Books = [];
            //Should this loading be done with dataManger?
            string[] files = Directory.GetFiles(configs.PathToLibrary!, "*.epub", SearchOption.AllDirectories).Order().ToArray();
            foreach (string file in files)
            {
                //Should be wrapper in a try-catch. Isn't because it's still not published and errors are easier to find like this.
                Books.Add(await EpubLoader.LoadMetadata(file));
                BookAdded.Invoke(this, EventArgs.Empty);
            }
        }

        public async void SaveAll()
        {

        }
    }
}
