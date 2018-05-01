using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

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

        public static EntityType GetEntityType<T>(this T model) where T: class, new()
        {
            return GetEntityType(typeof(T));
        }

        public static EntityType GetEntityType(this Type t)
        {
            var entityTypeAttr = t.GetTypeInfo().GetCustomAttributes<EntityTypeAttribute>().FirstOrDefault();
            if (entityTypeAttr == null)
            {
                throw new Exception("The model is missing entity type");
            }
            return entityTypeAttr.EntityType;
        }

        public static string GetValue(this IEnumerable<OptionModel> options, string key, string defaultValue = "")
        {
            var result = defaultValue;
            if (options == null || options.Count() <= 0)
            {
                return result;
            }
            var first = options.FirstOrDefault(o => o.Key == key);
            if (first == null)
            {
                return defaultValue;
            }
            return first.Value;
        }

        public static T GetValue<T>(this IEnumerable<OptionModel> options, string key, T defaultValue = default(T))
        {
            if (options == null || options.Count() <= 0)
            {
                return defaultValue;
            }

            var first = options.FirstOrDefault(o => o.Key == key);
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
