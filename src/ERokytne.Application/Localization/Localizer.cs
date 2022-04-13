using System.Reflection;

namespace ERokytne.Application.Localization
{
    public static partial class Localizer
    {
        private const string DefaultLanguageCode = "en";

        private static readonly LocalizationSource EntitiesSource = new();

        private static readonly LocalizationSource PropertiesSource = new();

        private static readonly LocalizationSource ExceptionsSource = new();

        private static readonly LocalizationSource MessagesSource = new();

        public static class Entities
        {
            public static string Get(Type type, bool defaultLanguage = false) =>
                EntitiesSource.Get(type.Name, defaultLanguage);

            public static string Get(string typeName, bool defaultLanguage = false) =>
                EntitiesSource.Get(typeName, defaultLanguage);
        }

        public static class Properties
        {
            public static string Get(PropertyInfo property, bool defaultLanguage = false)
            {
                var keys = property.DeclaringType == null
                    ? new[] { property.Name }
                    : new[] { $"{property.DeclaringType.Name}:{property.Name}", property.Name };

                foreach (var key in keys)
                {
                    if (PropertiesSource.TryGet(key, defaultLanguage, out var translation))
                    {
                        return translation;
                    }
                }

                return property.Name;
            }

            public static string Get<T>(string propertyName, bool defaultLanguage = false) =>
                Get(typeof(T).GetProperty(propertyName), defaultLanguage);
            
            public static string Get(string propertyName, bool defaultLanguage = false) =>
                PropertiesSource.TryGet(propertyName, defaultLanguage, out var translation)
                    ? translation
                    : propertyName;
        }

        public static class Messages
        {
            public static string Get(string errorCode, bool defaultLanguage = false, params object[] args) =>
                MessagesSource.Get(errorCode, defaultLanguage, args);
        }

        public static class Exceptions
        {
            public static string Get(int exceptionCode, bool defaultLanguage = false, params object[] args) =>
                ExceptionsSource.Get(exceptionCode.ToString(), defaultLanguage, args);
        }
        
        internal static object[] GetFormatArguments(IEnumerable<object> args, bool defaultLanguage)
        {
            return args.Select(arg => arg switch
            {
                Type type => Entities.Get(type, defaultLanguage),
                PropertyInfo propertyInfo => Properties.Get(propertyInfo, defaultLanguage),
                _ => arg
            }).ToArray();
        }
    }
}