using ModManager.Models;
using RestSharp;
using System.Text.Json;

namespace ModManager.DAL
{
    public static class ModrinthApi
    {
        readonly static RestClient _client = new ("https://api.modrinth.com/v2/");

        public static async Task<Search?> SearchAsync(string query, string? version = null, string? category = null)
        {
            var facets = new List<string>();

            if (category != null)
                facets.Add($"[\"categories:{category}\"]");
            if (version != null)
                facets.Add($"[\"versions:{version}\"]");
            facets.Add("[\"project_type:mod\"]");

            var request = new RestRequest($"search", Method.Get);
            request.AddParameter("query", query);
            request.AddParameter("facets", $"[{string.Join(", ",facets)}]");

            return await _client.GetAsync<Search>(request);
        }

        public static async Task<Project?> GetProjectAsync(string id)
        {
            var request = new RestRequest($"project/{id}", Method.Get);
            return await _client.GetAsync<Project>(request);
        }

        public static async Task<List<ModVersion>?> GetVersionsAsync(string id, string? version = null, string? loader = null)
        {
            var request = new RestRequest($"project/{id}/version", Method.Get);
            if (loader != null)
                request.AddParameter("loaders", loader);
            if (version != null)
                request.AddParameter("game_versions", version);

            return await _client.GetAsync<List<ModVersion>>(request);
        }

        public static async Task<byte[]?> DownloadMod(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Get);
            return await client.DownloadDataAsync(request);
        }
    }
}