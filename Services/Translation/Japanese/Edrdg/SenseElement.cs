﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maria.Services.Translation.Japanese.Edrdg
{
    //A bunch of information contained in the sense tag has been ignored for simplicity's sake. Some of them ARE important 
    //for a full implementation, but they are not essential for a basic, working one and so will be left for later.
    internal class SenseElement
    {
        public List<string> Glosses { get; private set; }

        public SenseElement(XElement element)
        {
            Glosses = new List<string>();
            var glosses = element.Elements("gloss");
            foreach (var gloss in glosses)
            {
                Glosses.Add(gloss.Value);
            }
        }
    }
}