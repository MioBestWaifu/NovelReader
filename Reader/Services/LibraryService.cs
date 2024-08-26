using Mio.Reader.Parsing.Loading;
using Mio.Reader.Parsing.Structure;

/* Unmerged change from project 'Reader (net8.0-windows10.0.19041.0)'
Before:
using System;
After:
using Mio.Reader.Utilitarians;
using System;
*/
using Mio.Reader.Utilitarians;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class LibraryService (ConfigurationsService configs,DataManagementService dataManager, ImageParsingService imageParser)
    {
        /*
         * The base64 of the covers will take a lot o memory for larger libraries. Should:
         * 1 - Have pagination
         * 2 - Only keep base64 in memory while that page of the library is being displayed
        */
        public List<BookInteraction> Books { get; private set; } = [];
        public event EventHandler BookAdded;

        public async void Initialize()
        {
            Books = await dataManager.GetSavedInteractions();
            Parallel.ForEach(Books, async (book) =>
            {
                BookLoader loader = BookLoader.GetLoader(book.Metadata.Path, configs, imageParser);
                await loader.LoadAndResizeCover(book.Metadata,440,660);
                BookAdded.Invoke(this, EventArgs.Empty);
            });

            //Should this loading be done with dataManger?
            string[] epubFiles = Directory.GetFiles(configs.PathToLibrary!, "*.epub", SearchOption.AllDirectories);
            string[] pdfFiles = Directory.GetFiles(configs.PathToLibrary!, "*.pdf", SearchOption.AllDirectories);
            string[] files = epubFiles.Concat(pdfFiles).Order().ToArray();

            foreach (string file in files)
            {
                //Not sure how efficient this is for large libraries.
                if (Books.Any(b => b.Metadata.Path == file))
                {
                    continue;
                }
                //Should be wrapped in a try-catch. Isn't because it's still not published and errors are easier to find like this.
                BookLoader loader = BookLoader.GetLoader(file, configs, imageParser);
                Books.Add(new BookInteraction(await loader.LoadMetadata(file)));
                BookAdded.Invoke(this, EventArgs.Empty);
            }
        }

        public async void SaveAll()
        {
            await dataManager.SaveInteractions(Books);
        }
    }
}
