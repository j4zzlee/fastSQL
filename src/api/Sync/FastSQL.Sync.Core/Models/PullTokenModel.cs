using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_index_pull_histories")]
    [EntityType(EntityType.PullResult)]
    public class PullTokenModel
    {
        public string TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public string PullResultStr { get; set; }
        public long LastUpdated { get; set; }

        public EntityType EntityType => EntityType.PullResult;

        public PullResult PullResult => string.IsNullOrWhiteSpace(PullResultStr) 
            ? null
            : JsonConvert.DeserializeObject<PullResult>(PullResultStr);
    }
}
