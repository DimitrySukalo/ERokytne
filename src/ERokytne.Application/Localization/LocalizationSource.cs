using ERokytne.Application.Extensions;

namespace ERokytne.Application.Localization
{
    internal class LocalizationSource : Dictionary<string, LocalizationEntry>
    {
        public LocalizationSource() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        internal string Get(string key, bool defaultLanguage, params object[] args) =>
            !TryGet(key, defaultLanguage, out var localizedString)
                ? key
                : localizedString.SafeFormat(Localizer.GetFormatArguments(args, defaultLanguage));
        
        internal bool TryGet(string key, bool defaultLanguage, out string translation)
        {
            if (TryGetValue(key, out var translations) && translations.TryGet(defaultLanguage, out translation))
            {
                return true;
            }

            translation = key;
            return false;
        }
    }
}