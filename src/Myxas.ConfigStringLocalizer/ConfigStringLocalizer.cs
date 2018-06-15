using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;


namespace Myxas.ConfigStringLocalizer
{
    public class ConfigStringLocalizer : IStringLocalizer
    {
        public ConfigStringLocalizer(IConfiguration config, CultureInfo withCulture = null)
        {
            _config = config;
            _withCulture = withCulture;

            ChangeToken.OnChange(() => _config.GetReloadToken(), LoadResources);

            LoadResources();
        }


        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return includeParentCultures
                ? _resources.Keys.Select(x => this[UnescapeKeyDelimiters(x)])
                : _resources
                    .Where(x => x.Value.ContainsKey(CurrentUICulture.Name))
                    .Select(x => new LocalizedString(UnescapeKeyDelimiters(x.Key), x.Value[CurrentUICulture.Name], false));
        }


        public IStringLocalizer WithCulture(CultureInfo culture)
            => new ConfigStringLocalizer(_config, culture);


        public LocalizedString this[string name] {
            get {
                var value = GetStringSafely(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }


        public LocalizedString this[string name, params object[] arguments] {
            get {
                var format = GetStringSafely(name);
                var value = String.Format(CurrentCulture, format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }


        private string GetStringSafely(string name, CultureInfo culture = null)
        {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (culture == null) {
                culture = CurrentUICulture;
            }

            if (!_resources.TryGetValue(EscapeKeyDelimiters(name), out var translations) || translations == null) {
                return null;
            }

            do {
                if (translations.TryGetValue(culture.Name, out var value)) {
                    return value;
                }

                if (culture.Equals(culture.Parent)) {
                    return null;
                }

                culture = culture.Parent;
            } while (true);
        }


        private void LoadResources()
        {
            var newResources = new Dictionary<string, Dictionary<string, string>>();
            foreach (var child in _config.GetChildren()) {
                var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var resource in child.GetChildren()) {
                    translations[resource.Key] = resource.Value;
                }
                newResources[child.Key] = translations;
            }
            _resources = newResources;
        }


        private static string EscapeKeyDelimiters(string key)
            => key.Replace(":", "&colon;");


        private static string UnescapeKeyDelimiters(string key)
            => key.Replace("&colon;", ":");


        private CultureInfo CurrentCulture => _withCulture ?? CultureInfo.CurrentCulture;
        private CultureInfo CurrentUICulture => _withCulture ?? CultureInfo.CurrentUICulture;


        private Dictionary<string, Dictionary<string, string>> _resources;


        private readonly IConfiguration _config;
        private readonly CultureInfo _withCulture;
    }
}
