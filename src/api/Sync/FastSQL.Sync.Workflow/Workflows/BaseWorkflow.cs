using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Workflows;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Workflows
{
    public abstract class BaseWorkflow : IWorkflow, INormalWorkflow
    {
        public abstract string Id { get; }

        public abstract int Version { get; }

        public abstract void Build(IWorkflowBuilder<object> builder);
    }

    public abstract class BaseWorkflow<T> : IWorkflow<T>, IGenericWorkflow
        where T : class, new()
    {
        public abstract string Id { get; }

        public abstract int Version { get; }

        public abstract void Build(IWorkflowBuilder<T> builder);
    }
}
