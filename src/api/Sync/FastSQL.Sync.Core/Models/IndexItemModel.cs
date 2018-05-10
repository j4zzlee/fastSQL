using FastSQL.Sync.Core.Enums;
using Newtonsoft.Json.Linq;
using st2forget.commons.datetime;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class IndexItemModel: Dictionary<string, object>
    {
        public static IndexItemModel FromObject(object i)
        {
            if (i.GetType() == typeof(IndexItemModel))
            {
                return i as IndexItemModel;
            }
            if (i.GetType() == typeof(JObject))
            {
                return FromJObject(i as JObject);
            }
            return FromJObject(JObject.FromObject(i));
        }

        public static IndexItemModel FromJObject(JObject i)
        {
            var result = new IndexItemModel();
            foreach (var key in i.Properties())
            {
                result.Add(key.Name, i.GetValue(key.Name).ToString());
            }

            if (result.ContainsKey("LastUpdated") && !result.ContainsKey("Last Updated Date"))
            {
                result.Add("Last Updated Date", long.Parse(result["LastUpdated"]?.ToString() ?? "0").UnixTimeToTime());
            }

            if (result.ContainsKey("State"))
            {
                ItemState state = (ItemState)int.Parse(result["State"].ToString()); // (ItemState)Enum.Parse(typeof(ItemState), result["State"].ToString());
                if (!result.ContainsKey("Changed"))
                {
                    result.Add("Changed", ((state & ItemState.Changed) > 0).ToString());
                }
                if (!result.ContainsKey("Removed"))
                {
                    result.Add("Removed", ((state & ItemState.Removed) > 0).ToString());
                }
                if (!result.ContainsKey("Invalid"))
                {
                    result.Add("Invalid", ((state & ItemState.Invalid) > 0).ToString());
                }
                if (!result.ContainsKey("Processed"))
                {
                    result.Add("Processed", ((state & ItemState.Processed) > 0).ToString());
                }
            }
            return result;
        }

        public static IndexItemModel FromDictionary(IDictionary<string, object> i)
        {
            var result = new IndexItemModel();
            foreach (var key in i.Keys)
            {
                result.Add(key, i[key]?.ToString());
            }

            if (result.ContainsKey("LastUpdated"))
            {
                result.Add("Last Updated Date", long.Parse(result["LastUpdated"]?.ToString() ?? "0").UnixTimeToTime());
            }

            if (result.ContainsKey("State"))
            {
                ItemState state = (ItemState)int.Parse(result["State"].ToString()); // (ItemState)Enum.Parse(typeof(ItemState), result["State"].ToString());
                result.Add("Changed", ((state & ItemState.Changed) > 0).ToString());
                result.Add("Removed", ((state & ItemState.Removed) > 0).ToString());
                result.Add("Invalid", ((state & ItemState.Invalid) > 0).ToString());
                result.Add("Processed", ((state & ItemState.Processed) > 0).ToString());
            }
            return result;
        }

        //public string Id
        //{
        //    get => ContainsKey("Id") ? this["Id"]?.ToString() : string.Empty;
        //}

        //public string SourceId
        //{
        //    get => ContainsKey("SourceId") ? this["SourceId"]?.ToString() : string.Empty;
        //}

        //public string DestinationId
        //{
        //    get => ContainsKey("DestinationId") ? this["DestinationId"]?.ToString() : string.Empty;
        //}

        //public string LastUpdatedDate
        //{
        //    get => ContainsKey("LastUpdated") ? this["LastUpdated"]?.ToString() : string.Empty;
        //}

        //public bool IsCreated { get; set; }
        //public bool IsChanged { get; set; }
        //public bool IsUpdated { get; set; }
        //public bool IsRemoved { get; set; }
        //public bool IsInvalid { get; set; }
    }
}
