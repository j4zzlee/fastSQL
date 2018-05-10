using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public interface IIndexModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string SourceProcessorId { get; set; }
        string DestinationProcessorId { get; set; }
        Guid SourceConnectionId { get; set; }
        Guid DestinationConnectionId { get; set; }
        EntityState State { get; set; }
        EntityType EntityType { get; }

        string SourceViewName { get; set; }
        string OldValueTableName { get; set; }
        string NewValueTableName { get; set; }
        string ValueTableName { get; set; }

        bool HasState(EntityState state);
        void RemoveState(EntityState state);
        void AddState(EntityState state);
    }
}
