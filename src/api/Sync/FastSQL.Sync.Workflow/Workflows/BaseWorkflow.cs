using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Workflows;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    public abstract class BaseWorkflow : BaseWorkflow<object>, IWorkflow, INormalWorkflow
    {
        public override bool IsGeneric => false;
    }

    public abstract class BaseWorkflow<T> : IWorkflow<T>, IGenericWorkflow
        where T : class, new()
    {
        public abstract string Id { get; }

        public abstract int Version { get; }
        public virtual bool IsGeneric => true;

        public abstract void Build(IWorkflowBuilder<T> builder);
        protected WorkflowMode Mode { get; set; } = WorkflowMode.None;
        protected IIndexModel IndexModel { get; set; }
        public void SetMode(WorkflowMode mode)
        {
            Mode = mode;
        }

        public void SetIndex(IIndexModel model)
        {
            IndexModel = model;
        }
    }
}
