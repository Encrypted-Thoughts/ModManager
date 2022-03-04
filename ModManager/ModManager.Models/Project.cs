using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Models
{
    public class Project
    {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] categories { get; set; }
        public string client_side { get; set; }
        public string server_side { get; set; }
        public string body { get; set; }
        public object body_url { get; set; }
        public string status { get; set; }
        public License license { get; set; }
        public string issues_url { get; set; }
        public string source_url { get; set; }
        public string wiki_url { get; set; }
        public string discord_url { get; set; }
        public Donation_Urls[] donation_urls { get; set; }
        public string project_type { get; set; }
        public int downloads { get; set; }
        public string icon_url { get; set; }
        public string id { get; set; }
        public string team { get; set; }
        public object moderator_message { get; set; }
        public DateTime published { get; set; }
        public DateTime updated { get; set; }
        public int followers { get; set; }
        public string[] versions { get; set; }
        public Gallery[] gallery { get; set; }
    }

    public class License
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Donation_Urls
    {
        public string id { get; set; }
        public string platform { get; set; }
        public string url { get; set; }
    }

    public class Gallery
    {
        public string url { get; set; }
        public bool featured { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime created { get; set; }
    }
}
