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
    }

    public interface INormalWorkflow: IBaseWorkflow
    {
    }

    public interface IGenericWorkflow : IBaseWorkflow
    {
    }
    
}
