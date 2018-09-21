using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace devoft.Core.System.Localization
{
    public interface ILocalizedResourceService
    {
        CultureInfo CurrentCulture { get; }
        Task SetCurrentCultureAsync(CultureInfo culture);
        T GetResource<T>(string key);
    }

}
