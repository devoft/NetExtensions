using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoft.System
{
    public class DictionaryObject : Dictionary<string, object>
    {
        public DictionaryObject()
        {
        }

        public DictionaryObject(object model)
        {
            foreach (var prop in model.GetType().GetProperties())
                this[prop.Name] = prop.GetValue(model);
        }

        public void CopyTo(object target)
        {
            var properties = target.GetType().GetProperties().Where(x => this.ContainsKey(x.Name));
            foreach (var propertyInfo in properties)
                propertyInfo.SetValue(target, this[propertyInfo.Name]);
        }
    }
}
