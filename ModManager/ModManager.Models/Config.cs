using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Models
{
    public class Config
    {
        public List<Mod> mods { get; set; } = new List<Mod>();
    }

    public class Mod
    {
        public string loader { get; set; }
        public string version { get; set; }
        public string id { get; set; }
        public string slug { get; set; }
        public string title { get; set; } 
        public string filename { get; set; }
        public string path { get; set; }
        public DateTime modified { get; set; }
    }

}
