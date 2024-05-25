using Maria.Common.Communication;
using Maria.Services.Translation.Japanese.Edrdg;
using Maria.Translation;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maria.Translation.Japanese
{
    //Should be on Recordkeeping. It may have japanese in the name, but it's function is dealing wih records.
    internal static class JapaneseDictionaryLoader
    {

        //Should determine the source (jmdict, names dict) once those other databases are implemented.
        public static List<ConversionEntry> LoadPossibleEntries(int index)
        {
            int file = Math.DivRem(index, 256, out int offset);
            byte[] data = File.ReadAllBytes($"{Constants.Paths.ToConvertedDictionary}{file}.bin");
            return MessagePackSerializer.Deserialize<List<List<ConversionEntry>>>(data)![offset];
        }
    }
}
