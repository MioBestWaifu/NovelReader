﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maria.Services.Communication.Commanding
{
    public class PrefixDefinition
    {
        [JsonInclude]
        public List<List<String>> Mutex { get; private set; }
    }
}