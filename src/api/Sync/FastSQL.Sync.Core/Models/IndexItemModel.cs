using FastSQL.Sync.Core.Enums;
using Newtonsoft.Json.Linq;
using DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FastSQL.Sync.Core.Models
{
    public class IndexItemModel: JObject
    {
        public static IndexItemModel FromJObject(JObject j)
        {
            var r = new IndexItemModel();
            foreach (var p in j.Properties())
            {
                r.Add(p.Name, j.GetValue(p.Name));
            }
            r.BuildProperties();
            return r;
        }
        public IndexItemModel BuildProperties()
        {
            if (ContainsKey("LastUpdated") && !ContainsKey("Last Updated Date"))
            {
                Add("Last Updated Date", long.Parse(this["LastUpdated"]?.ToString() ?? "0").UnixTimeToTime());
            }

            if (ContainsKey("State"))
            {
                ItemState state = (ItemState)int.Parse(this["State"].ToString()); // (ItemState)Enum.Parse(typeof(ItemState), result["State"].ToString());
                if (!ContainsKey("Changed"))
                {
                    Add("Changed", ((state & ItemState.Changed) > 0).ToString());
                }
                if (!ContainsKey("Removed"))
                {
                    Add("Removed", ((state & ItemState.Removed) > 0).ToString());
                }
                if (!ContainsKey("Invalid"))
                {
                    Add("Invalid", ((state & ItemState.Invalid) > 0).ToString());
                }
                if (!ContainsKey("Processed"))
                {
                    Add("Processed", ((state & ItemState.Processed) > 0).ToString());
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
            if (!(ContainsKey("State")))
            {
                return false;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());
            return (s & state) > 0;
        }

        public void SetState(ItemState state)
        {
            if (!(ContainsKey("State")))
            {
                return;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());

            this["State"] = JToken.FromObject(s == 0 ? state : s | state);
        }

        public void RemoveState(ItemState state)
        {
            if (!(ContainsKey("State") == true))
            {
                return;
            }

            var s = (ItemState)int.Parse(this["State"].ToString());

            this["State"] = JToken.FromObject(s == 0 ? s : (s | state) ^ state);
        }
    }
}
