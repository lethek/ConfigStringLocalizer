using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;


namespace Myxas.ConfigStringLocalizer
{

    public static class LocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddConfigLocalization(this IServiceCollection services)
        {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            AddConfigLocalization(services, setupAction: null);

            return services;
        }


        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="setupAction">
        /// An <see cref="Action{ConfigLocalizationOptions}"/> to configure the <see cref="ConfigLocalizationOptions"/>.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddConfigLocalization(this IServiceCollection services,
            Action<ConfigLocalizationOptions> setupAction)
        {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            // To enable unit testing
            services.TryAddSingleton<IStringLocalizerFactory, ConfigStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            if (setupAction != null) {
                services.Configure(setupAction);
            }

            return services;
        }
    }

}
