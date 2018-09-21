namespace devoft.Core.Patterns.Scoping
{
    public interface IPropertyNotificationScopeAspect : IScopeAspect
    {
        void Record(IPropertyChangedNotifier target, string propertyName);
    }
}
