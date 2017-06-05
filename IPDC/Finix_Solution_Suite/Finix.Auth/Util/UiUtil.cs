using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Util
{
    public static class UiUtil
    {

        public static Dictionary<string, string> GetOptionsForJqGridDdl<T>(List<T> items, string valueProperty, string displayProperty) where T : class
        {
            var type = items.First().GetType();
            var valuePropertyInfo = type.GetProperty(valueProperty);
            var displayPropertyInfo = type.GetProperty(displayProperty);

            var retVal = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var v = valuePropertyInfo.GetValue(item, null).ToString();
                var d = displayPropertyInfo.GetValue(item, null).ToString();
                retVal.Add(v, d);
            }
            return retVal;
        }

        public static List<KeyValuePair<int, string>> EnumToKeyVal<T>(bool orderbyValue = true)
        {
            var t = typeof(T);
            return orderbyValue
                ? Enum.GetNames(t)
                    .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
                    .ToList().OrderBy(x => x.Value).ToList()
                : Enum.GetNames(t)
                    .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
                    .ToList().OrderBy(x => x.Key).ToList();
        }
    }
}
