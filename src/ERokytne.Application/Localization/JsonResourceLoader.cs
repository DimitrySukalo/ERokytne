using System.Reflection;
using System.Text.Json;

namespace ERokytne.Application.Localization
{
    internal static class JsonResourceLoader
    {
        public static async Task<Dictionary<string, Dictionary<string, string>>> LoadResourceAsync(
            Assembly assembly, string resource)
        {
            using var stream = assembly.GetManifestResourceStream(resource);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resource} of assembly {assembly.FullName} not found");
            }

            return await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(stream);
        }
    }
}