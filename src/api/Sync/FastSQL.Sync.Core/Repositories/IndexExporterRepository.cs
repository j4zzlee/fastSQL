using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class IndexExporterRepository : BaseRepository
    {
        public IndexExporterRepository(DbConnection connection) : base(connection)
        {
        }

        public override void LinkOptions(string id, IEnumerable<OptionItem> options)
        {
            base.LinkOptions(id, EntityType.Exporter, options);
        }

        public override IEnumerable<OptionModel> LoadOptions(string entityId, IEnumerable<string> optionGroups = null)
        {
            return base.LoadOptions(entityId, EntityType.Exporter, optionGroups);
        }

        public override IEnumerable<OptionModel> LoadOptions(IEnumerable<string> entityIds, IEnumerable<string> optionGroups = null)
        {
            return base.LoadOptions(entityIds, EntityType.Exporter, optionGroups);
        }

        public override void UnlinkOptions(string id, IEnumerable<string> optionGroups = null)
        {
            base.UnlinkOptions(id, EntityType.Exporter, optionGroups);
        }
    }
}
