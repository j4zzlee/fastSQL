using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Core.ExtensionMethods
{
    public static class OptionsExtensions
    {
        public static string GetValue(this IEnumerable<OptionItem> options, string key, string defaultValue = "")
        {
            var result = defaultValue;
            if (options == null || options.Count() <= 0)
            {
                return result;
            }
            var first = options.FirstOrDefault(o => o.Name == key);
            if (first == null)
            {
                return defaultValue;
            }
            return first.Value;
        }

        public static T GetValue<T>(this IEnumerable<OptionItem> options, string key, T defaultValue = default(T))
        {
            if (options == null || options.Count() <= 0)
            {
                return defaultValue;
            }

            var first = options.FirstOrDefault(o => o.Name == key);
            if (first == null)
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(first.Value))
            {
                return default(T);
            }

            return (T)Convert.ChangeType(first.Value, typeof(T));
        }
    }
}
