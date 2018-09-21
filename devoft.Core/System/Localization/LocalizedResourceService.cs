using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading.Tasks;

namespace devoft.Core.System.Localization
{
    public class LocalizedResourceService : ILocalizedResourceService
    {
        private ResourceSettings _resources;

        public LocalizedResourceService(IOptions<ResourceSettings> resources) => _resources = resources.Value;

        public CultureInfo CurrentCulture { get; private set; }

        public Task SetCurrentCultureAsync(CultureInfo culture)
            => Task.Factory.StartNew(() =>
            {
                lock (_resources)
                {
                    var dict = _resources[culture.Name];
                    foreach (var entry in dict)
                        dict[entry.Key] = entry.Value;
                    CurrentCulture = culture;
                }
            });
        

        public T GetResource<T>(string key)
            => (_resources[CurrentCulture.Name].TryGetValue(key, out var result))
                        ? JsonConvert.DeserializeObject<T>(result)
                        : default(T);
    }


}
