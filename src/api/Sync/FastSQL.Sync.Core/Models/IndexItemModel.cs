using FastSQL.Sync.Core.Enums;
using Newtonsoft.Json.Linq;
using DateTimeExtensions;
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
            return result.BuildProperties();
        }

        public static IndexItemModel FromDictionary(IDictionary<string, object> i)
        {
            var result = new IndexItemModel();
            foreach (var key in i.Keys)
            {
                result.Add(key, i[key]?.ToString());
            }
            return result.BuildProperties();
            
        }

        private IndexItemModel BuildProperties()
        {
            if (this.ContainsKey("LastUpdated") && !this.ContainsKey("Last Updated Date"))
            {
                this.Add("Last Updated Date", long.Parse(this["LastUpdated"]?.ToString() ?? "0").UnixTimeToTime());
            }

            if (this.ContainsKey("State"))
            {
                ItemState state = (ItemState)int.Parse(this["State"].ToString()); // (ItemState)Enum.Parse(typeof(ItemState), result["State"].ToString());
                if (!this.ContainsKey("Changed"))
                {
                    this.Add("Changed", ((state & ItemState.Changed) > 0).ToString());
                }
                if (!this.ContainsKey("Removed"))
                {
                    this.Add("Removed", ((state & ItemState.Removed) > 0).ToString());
                }
                if (!this.ContainsKey("Invalid"))
                {
                    this.Add("Invalid", ((state & ItemState.Invalid) > 0).ToString());
                }
                if (!this.ContainsKey("Processed"))
                {
                    this.Add("Processed", ((state & ItemState.Processed) > 0).ToString());
                }
            }
            return this;
        }

        public string GetId()
        {
            return ContainsKey("Id") ? this["Id"]?.ToString() : string.Empty;
        }

        public string GetSourceId()
        {
            return ContainsKey("SourceId") ? this["SourceId"]?.ToString() : string.Empty;
        }

        public string GetDestinationId()
        {
            return ContainsKey("DestinationId") ? this["DestinationId"]?.ToString() : string.Empty;
        }

        public bool HasState(ItemState state)
        {
            if (!this.ContainsKey("State"))
            {
                return false;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());
            return (s & state) > 0;
        }

        public void SetState(ItemState state)
        {
            if (!this.ContainsKey("State"))
            {
                return;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());

            this["State"] = s == 0 ? state : s | state;
        }

        public void RemoveState(ItemState state)
        {
            if (!this.ContainsKey("State"))
            {
                return;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());

            this["State"] = s == 0 ? s : (s | state) ^ state;
        }
    }
}
