using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using System;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    public abstract class BaseWorkflow : BaseWorkflow<object>, IWorkflow, INormalWorkflow
    {
        public override bool IsGeneric => false;
    }

    public abstract class BaseWorkflow<T> : IWorkflow<T>, IGenericWorkflow, IDisposable
        where T : class, new()
    {
        public abstract string Id { get; }

        public abstract int Version { get; }
        public virtual bool IsGeneric => true;

        public abstract void Build(IWorkflowBuilder<T> builder);

        public ResolverFactory ResolverFactory { get; set; } 
        public RepositoryFactory RepositoryFactory { get; set; }

        public virtual void Dispose()
        {
            RepositoryFactory.Release(this);
        }
    }
}
