using System;

using Microsoft.Extensions.Configuration;


namespace Myxas.ConfigStringLocalizer
{

    /// <summary>Provides programmatic configuration for localization.</summary>
    public class ConfigLocalizationOptions
    {
        public IConfiguration Configuration { get; set; }
        public StringComparer KeyComparer { get; set; }
        public Func<string, string> KeyEncoder { get; set; }
        public Func<string, string> KeyDecoder { get; set; }
    }

}
