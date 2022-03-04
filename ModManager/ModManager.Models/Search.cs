namespace ModManager.Models
{
    public class Search
    {
        public Hit[] hits { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public int total_hits { get; set; }
    }
    public class Hit
    {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] categories { get; set; }
        public string client_side { get; set; }
        public string server_side { get; set; }
        public string project_type { get; set; }
        public int downloads { get; set; }
        public string icon_url { get; set; }
        public string project_id { get; set; }
        public string author { get; set; }
        public string[] versions { get; set; }
        public int follows { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public string latest_version { get; set; }
        public string license { get; set; }
        public string[] gallery { get; set; }
    }

}
