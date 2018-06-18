using System;
using System.Globalization;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Myxas.ConfigStringLocalizer.Tools;

using Xunit;


namespace Myxas.ConfigStringLocalizer
{

    public class ConfigStringLocalizerTests
    {
        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_NullResourceKey_ThrowsArgumentNullException(ConfigType config)
        {
            var localizer = NewLocalizer(config);
            Assert.Throws<ArgumentNullException>(() => localizer[null]);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_HaveTranslation_ReturnsExactString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("pt-BR");
            var value = localizer["Close"];
            Assert.Equal("Fechar", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_HaveTranslationIgnoreCase_ReturnsExactString(ConfigType config)
        {
            var localizer = NewLocalizer(config, StringComparer.CurrentCultureIgnoreCase);

            SetCulture("pt-BR");
            var value = localizer["close"];
            Assert.Equal("Fechar", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_HaveParentTranslation_ReturnsParentString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("de-DE");
            var value = localizer["Colour"];
            Assert.Equal("Farbe", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("zh-SG");
            value = localizer["Colour"];
            Assert.Equal("颜色", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_HaveInvariantTranslation_ReturnsInvariantString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("en-AU");
            var value = localizer["Invariant"];
            Assert.Equal("Invariant", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_MissingTranslation_ReturnsDefaultKey(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("zh");
            var value = localizer["Colour"];
            Assert.Equal("Colour", value);
            Assert.True(value.ResourceNotFound);

            SetCulture("fr");
            value = localizer["Colour"];
            Assert.Equal("Colour", value);
            Assert.True(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_MissingResource_ReturnsDefaultKey(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("en-AU");
            var value = localizer["No resource key with this name"];
            Assert.Equal("No resource key with this name", value);
            Assert.True(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_TranslationIsEscaped_ReturnsString(ConfigType config)
        {
            var localizer = NewLocalizer(config);
            SetCulture("id");
            var value = localizer["Branch to: {{branchQuestionName}}"];
            Assert.Equal("Loncat ke pertanyaan: {{branchQuestionName}}", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_TranslationIsNull_ReturnsEmptyString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("en-NZ");
            var value = localizer["NullValue"];
            Assert.Equal(String.Empty, value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetStringFormatted_HaveTranslation_ReturnsString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            var date = new DateTime(2018, 6, 15);

            SetCulture("en-AU");
            var value = localizer["CreatedOn", date];
            Assert.Equal("Created on 2018-06-15 00:00:00Z", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("hi");
            value = localizer["CreatedOn", date];
            Assert.Equal("15-06-2018 को बनाया गया", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetString_MergedOverrides_ReturnsString(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            SetCulture("en-AU");
            var value = localizer["Overridden"];
            Assert.Equal("Overriding an earlier config", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("en-US");
            value = localizer["Overridden"];
            Assert.Equal("Missing from an earlier config", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("en-NZ");
            value = localizer["Overridden"];
            Assert.Equal("This fallback should be found", value);
            Assert.False(value.ResourceNotFound);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetAllStrings_WithoutParentCulture_ReturnsSpecificCultureStrings(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            var expected = new[] {
                new LocalizedString("Artist", "Artista", false),
                new LocalizedString(
                    "Branch to: {{branchQuestionName}}", "Ramifique para: {{branchQuestionName}}", false
                ),
                new LocalizedString("Close", "Fechar", false)
            };

            SetCulture("pt-BR");
            var values = localizer.GetAllStrings(includeParentCultures: false);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void GetAllStrings_WithParentCulture_ReturnsAllStrings(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            var expected = new[] {
                new LocalizedString("Artist", "Artista", false),
                new LocalizedString(
                    "Branch to: {{branchQuestionName}}", "Ramifique para: {{branchQuestionName}}", false
                ),
                new LocalizedString("Close", "Fechar", false),
                new LocalizedString("Colour", "Cor", false),
                new LocalizedString("CreatedOn", "CreatedOn", true),
                new LocalizedString("Invariant", "Invariant", false),
                new LocalizedString("NullValue", "NullValue", true),
                new LocalizedString("Overridden", "Overridden", true)
            };

            SetCulture("pt-BR");
            var values = localizer.GetAllStrings(includeParentCultures: true);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void WithCultureGetString_HaveParentTranslation_ReturnsParentString(ConfigType config)
        {
            var localizer = NewLocalizer(config);
            var cnLocalizer = localizer.WithCulture(new CultureInfo("zh-CN"));
            var value = cnLocalizer["Artist"];
            Assert.Equal("歌手", value);
        }


        [Theory]
        [MemberData(nameof(GetConfigs))]
        public void WithCultureGetAllStrings_WithoutParentCulture_ReturnsSpecificCultureStrings(ConfigType config)
        {
            var localizer = NewLocalizer(config);

            var expected = new[] {
                new LocalizedString("Colour", "Colour", false),
                new LocalizedString("Overridden", "Overriding an earlier config", false)
            };

            var auLocalizer = localizer.WithCulture(new CultureInfo("en-AU"));
            var values = auLocalizer.GetAllStrings(includeParentCultures: false);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        private static void SetCulture(string cultureName)
        {
            var cultureInfo = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }


        private IStringLocalizer NewLocalizer(ConfigType configType, StringComparer comparer = null)
        {
            IConfiguration config = null;
            switch (configType) {
                case ConfigType.Json:
                    config = JsonConfig;
                    break;
                case ConfigType.Ini:
                    config = IniConfig;
                    break;
            }

            var options = Options.Create(
                new ConfigLocalizationOptions {
                    Configuration = config,
                    KeyComparer = comparer
                }
            );

            var factory = new ConfigStringLocalizerFactory(options);
            return factory.Create(typeof(IStringLocalizer));
        }


        public static TheoryData<ConfigType> GetConfigs
            => new TheoryData<ConfigType> {
                ConfigType.Json,
                ConfigType.Ini
            };


        private static readonly IConfiguration JsonConfig
            = new ConfigurationBuilder()
                .AddJsonFile(AppContext.BaseDirectory + "/Resources/Tests1.json")
                .AddJsonFile(AppContext.BaseDirectory + "/Resources/Tests2.json")
                .Build();


        private static readonly IConfiguration IniConfig
            = new ConfigurationBuilder()
                .AddIniFile(AppContext.BaseDirectory + "/Resources/Tests1.ini")
                .AddIniFile(AppContext.BaseDirectory + "/Resources/Tests2.ini")
                .Build();


        private static readonly LocalizedStringEqualityComparer LocalizedStringEqualityComparer
            = new LocalizedStringEqualityComparer();


        public enum ConfigType
        {
            Json,
            Ini
        }
    }

}
