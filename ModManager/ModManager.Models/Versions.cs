using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Models
{

    public class VersionContainer
    {
        public ModVersion[] versions { get; set; }
    }

    public class ModVersion
    {
        public string id { get; set; }
        public string project_id { get; set; }
        public string author_id { get; set; }
        public bool featured { get; set; }
        public string name { get; set; }
        public string version_number { get; set; }
        public string changelog { get; set; }
        public object changelog_url { get; set; }
        public DateTime date_published { get; set; }
        public int downloads { get; set; }
        public string version_type { get; set; }
        public ModFile[] files { get; set; }
        public object[] dependencies { get; set; }
        public string[] game_versions { get; set; }
        public string[] loaders { get; set; }
    }

    public class ModFile
    {
        public Hashes hashes { get; set; }
        public string url { get; set; }
        public string filename { get; set; }
        public bool primary { get; set; }
    }

    public class Hashes
    {
        public string sha512 { get; set; }
        public string sha1 { get; set; }
    }

}
