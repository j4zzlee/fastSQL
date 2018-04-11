using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FastSQL.Sync.Core.ExtensionMethods
{
    public static class ModelExtensions
    {
        public static string GetTableName<T>(this T model) where T : class, new()
        {
            return GetTableName(typeof(T));
        }
        public static string GetTableName(this Type t)
        {
            var tableAttr = t.GetTypeInfo().GetCustomAttributes<TableAttribute>().FirstOrDefault();
            var tableName = t.Name;
            if (tableAttr != null && !string.IsNullOrWhiteSpace(tableAttr.Name))
            {
                tableName = tableAttr.Name;
            }
            return tableName;
        }

        public static string GetKeyColumnName<T>(this T model) where T: class, new()
        {
            return GetKeyColumnName(typeof(T));
        }

        public static string GetKeyColumnName(this Type t)
        {
            return t
                .GetProperties()
                .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
                .Name;
        }
    }
}
