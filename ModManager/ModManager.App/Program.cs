using CommandLine;
using ModManager.Models;
using Newtonsoft.Json;

namespace ModManager.App
{
    public class Options
    {
        [Option('a', "add", Required = false, HelpText = "Add a new mod to the config.")]
        public string? Add { get; set; }

        [Option('u', "update", Required = false, HelpText = "Update specified mod.")]
        public string? Update { get; set; }

        [Option("updateAll", Required = false, Default = false, HelpText = "Update all mods.")]
        public bool UpdateAll { get; set; }

        [Option('v', "version", Required = false, HelpText = "Minecraft version of the mod.")]
        public string? Version { get; set; }

        [Option('l', "loader", Required = false, HelpText = "Loader type of the mod. (Fabric or Forge)")]
        public string? Loader { get; set; }

        [Option('p', "path", Required = false, HelpText = "Path to the folder that will contain the mod.")]
        public string? Path { get; set; }

        [Option("list", Required = false, HelpText = "Get a list of all managed mods.")]
        public bool List { get; set; }

        [Option('r', "remove", Required = false, HelpText = "Remove the specified mod.")]
        public string? Remove { get; set; }
    }

    public static class ModManagerApp
    {
        private const string _configName = "config.json";

        public static void Main(string[] args)
        {
            if (!File.Exists(_configName))
                File.Create(_configName);

            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_configName)) ?? new Config();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (!string.IsNullOrWhiteSpace(o.Add))
                    {
                        if (string.IsNullOrWhiteSpace(o.Path))
                        {
                            Console.WriteLine("Please enter a folder path at which to place the mod using the -path argument.");
                            return;
                        }

                        foreach (var mod in config.mods)
                        {
                            if (Path.Combine(mod.path) == Path.Combine(o.Path) && (mod.slug == o.Add || mod.id == o.Add))
                            {
                                Console.WriteLine($"This mod had already been added at this path. Please use the -update arguement instead.");
                                return;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(o.Version))
                        {
                            Console.WriteLine($"Please enter a minecraft version number (1.17, 1.18.1, etc) using the -version argument.");
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(o.Loader))
                        {
                            Console.WriteLine("Please enter a mod loader type (Fabric or Forge) using the -version argument.");
                            return;
                        }

                        AddMod(o.Add, o.Path, o.Version, o.Loader, config);
                    }
                    else if (!string.IsNullOrWhiteSpace(o.Update))
                    {
                        UpdateMod(o.Update, config);
                    }
                    else if (o.UpdateAll)
                    {
                        foreach (var mod in config.mods)
                        {
                            if (string.IsNullOrWhiteSpace(o.Path) || Path.Combine(mod.path) == Path.Combine(o.Path))
                                UpdateMod(mod.slug, config);
                        }
                    }
                    else if (o.List)
                    {
                        var found = false;
                        foreach (var mod in config.mods)
                        {
                            if (string.IsNullOrWhiteSpace(o.Path) || Path.Combine(mod.path) == Path.Combine(o.Path))
                            {
                                Console.WriteLine($"{mod.title} slug:{mod.slug} id:{mod.id} updated:{mod.modified}");
                                found = true;
                            }
                        }
                        if (!found)
                            Console.WriteLine("No managed mods found.");
                    }
                    else if (!string.IsNullOrWhiteSpace(o.Remove))
                    {
                        for (var i=0; i < config.mods.Count(); i++)
                        {
                            if (string.IsNullOrWhiteSpace(o.Path) || Path.Combine(config.mods[i].path) == Path.Combine(o.Path))
                            {
                                if (config.mods[i].slug == o.Remove || config.mods[i].id == o.Remove)
                                {
                                    var fullpath = Path.Combine(config.mods[i].path, config.mods[i].filename);
                                    if (File.Exists(fullpath))
                                        File.Delete(fullpath);

                                    config.mods.RemoveAt(i);

                                    File.WriteAllText(_configName, JsonConvert.SerializeObject(config, Formatting.Indented));
                                    break;
                                }
                            }
                        }
                    }
                });
        }

        private static void AddMod(string id, string path, string version, string loader, Config config)
        {

            var project = DAL.ModrinthApi.GetProjectAsync(id).Result;
            if (project == null)
            {
                Console.WriteLine($"Unable to find a project matching {id}");
                return;
            }

            var versions = DAL.ModrinthApi.GetVersionsAsync(id, version, loader).Result;
            if (versions == null)
            {
                Console.WriteLine($"Unable to retrieve versions for {id} {version} {loader}");
                return;
            }

            var latest = versions.OrderByDescending(v => v.date_published).FirstOrDefault();

            if (latest == null)
            {
                Console.WriteLine($"No versions found for {id} {version} {loader}");
                return;
            }

            var file = latest.files.FirstOrDefault();

            if (file != null)
            {
                var response = DAL.ModrinthApi.DownloadMod(file.url).Result;
                if (response != null)
                {
                    File.WriteAllBytes(Path.Combine(path,file.filename), response);

                    config.mods.Add(new Mod()
                    {
                        version = version,
                        loader = loader,
                        path = path,
                        filename = file.filename,
                        title = project.title,
                        id = project.id,
                        slug = project.slug,
                        modified = latest.date_published
                    });
                }
            }
            else
            {
                Console.WriteLine($"No files listed for {id} {version} {loader}");
                return;
            }

            File.WriteAllText(_configName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        private static void UpdateMod(string id, Config config)
        {
            foreach (var mod in config.mods)
            {
                if (mod.slug == id)
                {
                    var project = DAL.ModrinthApi.GetProjectAsync(id).Result;
                    var versions = DAL.ModrinthApi.GetVersionsAsync(id, mod.version, mod.loader).Result;

                    if (versions == null)
                    {
                        Console.WriteLine($"Unable to retrieve versions for {mod.slug} {mod.version} {mod.loader}");
                        return;
                    }

                    var latest = versions.OrderByDescending(v => v.date_published).FirstOrDefault();

                    if (latest == null)
                    {
                        Console.WriteLine($"No versions found for {mod.slug} {mod.version} {mod.loader}");
                        return;
                    }

                    if (latest.date_published > mod.modified)
                    {
                        var file = latest.files.FirstOrDefault();

                        if (file != null)
                        {
                            var response = DAL.ModrinthApi.DownloadMod(file.url).Result;
                            if (response != null)
                            {
                                File.WriteAllBytes(Path.Combine(mod.path, file.filename), response);

                                if (mod.filename != file.filename)
                                {
                                    var fullpath = Path.Combine(mod.path, mod.filename);
                                    if (File.Exists(fullpath))
                                        File.Delete(fullpath);
                                }

                                mod.filename = file.filename;
                                mod.modified = latest.date_published;

                                Console.WriteLine($"Updated {mod.title} - version: {mod.version} filename: {mod.filename} date_modified: {mod.modified}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No files listed for {mod.slug} {mod.version} {mod.loader}");
                            return;
                        }
                    }
                }
            }

            File.WriteAllText(_configName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}
