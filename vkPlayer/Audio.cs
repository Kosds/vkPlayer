﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vkPlayer
{
    class Audio
    {
        public int aid {get;set;}
        public int owner_id { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
        public string url { get; set; }
        public string lyrics_id { get; set; }
        public int genre { get; set; }
    }
}
