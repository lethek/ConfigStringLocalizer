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
        public ConfigStringLocalizerTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Resources/Tests1.json")
                .AddJsonFile("Resources/Tests2.json")
                .Build();

            var options = Options.Create(
                new ConfigLocalizationOptions {
                    Configuration = config
                }
            );

            Factory = new ConfigStringLocalizerFactory(options);
            Localizer = Factory.Create(typeof(IStringLocalizer));
        }


        [Fact]
        public void GetString_NullResourceKey_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Localizer[null]);
        }


        [Fact]
        public void GetString_HaveTranslation_ReturnsExactString()
        {
            SetCulture("pt-BR");
            var value = Localizer["Close"];
            Assert.Equal("Fechar", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_HaveParentTranslation_ReturnsParentString()
        {
            SetCulture("de-DE");
            var value = Localizer["Colour"];
            Assert.Equal("Farbe", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("zh-SG");
            value = Localizer["Colour"];
            Assert.Equal("颜色", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_HaveInvariantTranslation_ReturnsInvariantString()
        {
            SetCulture("en-AU");
            var value = Localizer["Invariant"];
            Assert.Equal("Invariant", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_MissingTranslation_ReturnsDefaultKey()
        {
            SetCulture("zh");
            var value = Localizer["Colour"];
            Assert.Equal("Colour", value);
            Assert.True(value.ResourceNotFound);

            SetCulture("fr");
            value = Localizer["Colour"];
            Assert.Equal("Colour", value);
            Assert.True(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_MissingResource_ReturnsDefaultKey()
        {
            SetCulture("en-AU");
            var value = Localizer["No resource key with this name"];
            Assert.Equal("No resource key with this name", value);
            Assert.True(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_TranslationIsEscaped_ReturnsString()
        {
            SetCulture("id");
            var value = Localizer["Branch to: {{branchQuestionName}}"];
            Assert.Equal("Loncat ke pertanyaan: {{branchQuestionName}}", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_TranslationIsNull_ReturnsEmptyString()
        {
            SetCulture("en-NZ");
            var value = Localizer["NullValue"];
            Assert.Equal(String.Empty, value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetStringFormatted_HaveTranslation_ReturnsString()
        {
            var date = new DateTime(2018, 6, 15);

            SetCulture("en-AU");
            var value = Localizer["CreatedOn", date];
            Assert.Equal("Created on 2018-06-15 00:00:00Z", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("hi");
            value = Localizer["CreatedOn", date];
            Assert.Equal("15-06-2018 को बनाया गया", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetString_MergedOverrides_ReturnsString()
        {
            SetCulture("en-AU");
            var value = Localizer["Overridden"];
            Assert.Equal("Overriding an earlier config", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("en-US");
            value = Localizer["Overridden"];
            Assert.Equal("Missing from an earlier config", value);
            Assert.False(value.ResourceNotFound);

            SetCulture("en-NZ");
            value = Localizer["Overridden"];
            Assert.Equal("This fallback should be found", value);
            Assert.False(value.ResourceNotFound);
        }


        [Fact]
        public void GetAllStrings_WithoutParentCulture_ReturnsSpecificCultureStrings()
        {
            var expected = new[] {
                new LocalizedString("Artist", "Artista", false),
                new LocalizedString(
                    "Branch to: {{branchQuestionName}}", "Ramifique para: {{branchQuestionName}}", false
                ),
                new LocalizedString("Close", "Fechar", false)
            };

            SetCulture("pt-BR");
            var values = Localizer.GetAllStrings(includeParentCultures: false);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        [Fact]
        public void GetAllStrings_WithParentCulture_ReturnsAllStrings()
        {
            var expected = new[] {
                new LocalizedString("Artist", "Artista", false),
                new LocalizedString("Branch to: {{branchQuestionName}}", "Ramifique para: {{branchQuestionName}}", false),
                new LocalizedString("Close", "Fechar", false),
                new LocalizedString("Colour", "Cor", false),
                new LocalizedString("CreatedOn", "CreatedOn", true),
                new LocalizedString("Invariant", "Invariant", false),
                new LocalizedString("NullValue", "NullValue", true),
                new LocalizedString("Overridden", "Overridden", true)
            };

            SetCulture("pt-BR");
            var values = Localizer.GetAllStrings(includeParentCultures: true);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        [Fact]
        public void WithCultureGetString_HaveParentTranslation_ReturnsParentString()
        {
            var cnLocalizer = Localizer.WithCulture(new CultureInfo("zh-CN"));
            var value = cnLocalizer["Artist"];
            Assert.Equal("歌手", value);
        }


        [Fact]
        public void WithCultureGetAllStrings_WithoutParentCulture_ReturnsSpecificCultureStrings()
        {
            var expected = new[] {
                new LocalizedString("Colour", "Colour", false),
                new LocalizedString("Overridden", "Overriding an earlier config", false)
            };

            var auLocalizer = Localizer.WithCulture(new CultureInfo("en-AU"));
            var values = auLocalizer.GetAllStrings(includeParentCultures: false);
            Assert.Equal(expected, values.ToArray(), LocalizedStringEqualityComparer);
        }


        private IStringLocalizerFactory Factory { get; }
        private IStringLocalizer Localizer { get; }


        private static void SetCulture(string cultureName)
        {
            var cultureInfo = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }


        private static readonly LocalizedStringEqualityComparer LocalizedStringEqualityComparer =
            new LocalizedStringEqualityComparer();
    }

}
