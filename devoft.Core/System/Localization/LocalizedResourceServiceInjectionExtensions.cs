using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

namespace devoft.Core.System.Localization
{
    public static class LocalizedResourceServiceInjectionExtensions
    {
        public static IServiceCollection AddLocalizedResources(this IServiceCollection serviceCollection)
            => serviceCollection.AddSingleton<ILocalizedResourceService, LocalizedResourceService>();
    }


}
