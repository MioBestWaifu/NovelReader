﻿using MessagePack;
using Mio.Translation.Japanese.Edrdg;

namespace Mio.Translation
{
    [MessagePackObject]
    public class ConversionEntry
    {
        [Key(0)]
        public string Key { get; set; }
        [Key(1)]
        public EdrdgEntry Value { get; set; }

        [SerializationConstructor]
        public ConversionEntry(string key, EdrdgEntry value)
        {
            Key = key;
            Value = value;
        }
    }
}