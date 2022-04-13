using System.Reflection;

namespace ERokytne.Application.Localization
{
    public static partial class Localizer
    {
        /// <summary>
        /// Loads resources from assemblies
        /// Resources should be places in folder 'Resources/Localization' and have type 'Embedded Resource'
        /// </summary>
        /// <param name="assemblies"></param>
        public static async Task LoadFromAssembliesAsync(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                await LoadFromAssemblyAsync(assembly);
            }

            var currentAssembly = typeof(Localizer).Assembly;
            if (!assemblies.Contains(currentAssembly))
            {
                await LoadFromAssemblyAsync(currentAssembly);
            }
        }

        private static async Task LoadFromAssemblyAsync(Assembly assembly)
        {
            foreach (var file in assembly.GetManifestResourceNames())
            {
                switch (file)
                {
                    case var _ when file.EndsWith("Resources.Localization.Entities.json"):
                        LoadLocalization(await JsonResourceLoader.LoadResourceAsync(assembly, file), EntitiesSource);
                        break;
                    case var _ when file.EndsWith("Resources.Localization.Properties.json"):
                        LoadLocalization(await JsonResourceLoader.LoadResourceAsync(assembly, file), PropertiesSource);
                        break;
                    case var _ when file.EndsWith("Resources.Localization.Exceptions.json"):
                        LoadLocalization(await JsonResourceLoader.LoadResourceAsync(assembly, file), ExceptionsSource);
                        break;
                    case var _ when file.EndsWith("Resources.Localization.Messages.json"):
                        LoadLocalization(await JsonResourceLoader.LoadResourceAsync(assembly, file), MessagesSource);
                        break;
                }
            }
        }

        private static void LoadLocalization(Dictionary<string, Dictionary<string, string>> source,
            LocalizationSource destination)
        {
            foreach (var (s, dictionary) in source)
            {
                if (!destination.TryGetValue(s, out var localizationEntry))
                {
                    localizationEntry = new LocalizationEntry();
                    destination.Add(s, localizationEntry);
                }

                foreach (var (key, value) in dictionary
                    .Where(sourceEntry => !localizationEntry.ContainsKey(sourceEntry.Key)))
                {
                    localizationEntry.Add(key, value);
                }
            }
        }
    }
}