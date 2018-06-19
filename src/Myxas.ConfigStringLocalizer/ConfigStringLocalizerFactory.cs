using System;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;


namespace Myxas.ConfigStringLocalizer
{

    public class ConfigStringLocalizerFactory : IStringLocalizerFactory
    {
        public ConfigStringLocalizerFactory(IOptions<ConfigLocalizationOptions> localizationOptions)
        {
            _options = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }


        public IStringLocalizer Create(Type resourceSource)
            => new ConfigStringLocalizer(
                _options.Value.Configuration,
                _options.Value.KeyComparer,
                _options.Value.KeyEncoder,
                _options.Value.KeyDecoder
            );


        public IStringLocalizer Create(string baseName, string location)
            => new ConfigStringLocalizer(
                _options.Value.Configuration,
                _options.Value.KeyComparer,
                _options.Value.KeyEncoder,
                _options.Value.KeyDecoder
            );


        private readonly IOptions<ConfigLocalizationOptions> _options;
    }

}
