using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Xunit;


namespace Myxas.ConfigStringLocalizer
{

    public class ConfigStringLocalizerFactoryTests
    {
        public ConfigStringLocalizerFactoryTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Resources/Tests1.json")
                .Build();

            var options = Options.Create(
                new ConfigLocalizationOptions {
                    Configuration = config
                }
            );

            Factory = new ConfigStringLocalizerFactory(options);
        }


        [Fact]
        public void CreateLocalizer_DefaultState_ReturnsNewLocalizer()
        {
            var localizer = Factory.Create(null);
            Assert.NotNull(localizer);

            localizer = Factory.Create(typeof(IStringLocalizer));
            Assert.NotNull(localizer);

            localizer = Factory.Create(baseName: null, location: null);
            Assert.NotNull(localizer);
        }


        private IStringLocalizerFactory Factory { get; }
    }

}
