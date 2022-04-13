using System.Globalization;

namespace ERokytne.Application.Localization
{
    internal class LocalizationEntry : Dictionary<string, string>
    {
        private const string DefaultLanguageCode = "uk";

        public LocalizationEntry() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        internal bool TryGet(bool defaultLanguage, out string translation)
        {
            foreach (var languageCode in GetLanguageCodes(defaultLanguage))
            {
                if (TryGetValue(languageCode, out translation))
                {
                    return true;
                }
            }

            translation = default;
            return false;
        }

        private static IEnumerable<string> GetLanguageCodes(bool defaultLanguage) =>
            defaultLanguage
                ? new[] { DefaultLanguageCode }
                : new[] { CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, DefaultLanguageCode };
    }
}