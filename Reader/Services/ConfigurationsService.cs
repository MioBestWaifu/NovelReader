using Mio.Reader.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Services
{
    public class ConfigurationsService
    {
        public string PathToUnidic { get; set; }
        //Multiple library folders to be implemented eventually, should be a list.
        public string PathToLibrary { get; set; }

        public ConfigurationsService()
        {
#if WINDOWS
            PathToUnidic = Path.Combine(AppContext.BaseDirectory, "Unidic");
#else
            PathToUnidic = Path.Combine(FileSystem.AppDataDirectory,"Unidic");
#endif
            EpubParser.Configs = this;
        }

    }
}
