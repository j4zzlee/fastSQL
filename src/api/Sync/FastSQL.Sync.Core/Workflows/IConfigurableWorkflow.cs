using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Core.Workflows
{
    public interface IBaseWorkflow
    {
        string Id { get; }
        int Version { get; }
        void SetMode(WorkflowMode mode);
        void SetIndex(IIndexModel model);
        bool IsGeneric { get; }
    }

    public interface INormalWorkflow: IBaseWorkflow
    {
    }

    public interface IGenericWorkflow : IBaseWorkflow
    {
    }
    
}
