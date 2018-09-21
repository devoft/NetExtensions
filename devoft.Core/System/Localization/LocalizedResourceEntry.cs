namespace devoft.Core.System.Localization
{
    public class LocalizedResourceEntry
    {
        public LocalizedResourceEntry(ILocalizedResourceService service, string key)
        {
            DisplayText = service.GetResource<string>(key);
        }

        public string Key { get; }
        public string DisplayText { get; }
    }

}
