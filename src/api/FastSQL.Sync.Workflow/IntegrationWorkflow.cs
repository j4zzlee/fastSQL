using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Workflow.Steps;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow
{
    public class Hello : StepBody
    {
        private ILogger _logger;
        public Hello(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _logger.Information("Begin Integration Workflow.");
            return ExecutionResult.Next();
        }
    }

    public class Goodbye : StepBody
    {
        private ILogger _logger;
        public Goodbye(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _logger.Information("End Integration Workflow.");
            return ExecutionResult.Next();
        }
    }

    public class PullSequenceStepData
    {
        public IEnumerable<IIndexModel> IndexModels { get; set; }
        public int Counter { get; set; }
    }

    public class PullParallelStepData
    {
        public IEnumerable<IIndexModel> IndexModels { get; set; }
    }

    public class MainWorkflowData
    {
        public MainWorkflowData()
        {
            PullSequence = new PullSequenceStepData();
            PullParallel = new PullParallelStepData();
        }
        public IEnumerable<object> Configurations { get; set; }
        public PullSequenceStepData PullSequence { get; set; }
        public PullParallelStepData PullParallel { get; set; }
    }

    public class IntegrationWorkflow : IWorkflow<MainWorkflowData>
    {
        public string Id => "IntegrationWorkflow";

        public int Version => 1;

        public IntegrationWorkflow()
        {
        }

        // The service is always have 4 tasks in general: Pull, Queue, Push and Report
        public void Build(IWorkflowBuilder<MainWorkflowData> builder)
        {
            var steps = builder.StartWith<Hello>()
                    .Then<GetConfigurationsStep>()
                    .Output(d => d.Configurations, step => step.Configurations)
                    .Parallel()
                    .Do(then => then.StartWith(x => { })
                        .While(d => true)
                        .Do(OnPull)
                    )
                    .Do(then => then.StartWith(x => { })
                        .While(d => true)
                        .Do(OnQueue)
                    )
                    .Do(then => then.StartWith(x => { })
                        .While(d => true)
                        .Do(OnPush)
                    )
                    .Do(then => then.StartWith(x => { })
                        .While(d => true)
                        .Do(OnReport)
                    )
                    .Join();

            steps.Then<Goodbye>();
        }

        private void OnPull(IWorkflowBuilder<MainWorkflowData> builder)
        {
            var p = builder.StartWith(x => { })
                .Parallel()
                // Pull in sequence
                .Do(then => then.StartWith<PullInSequenceStep>()
                            .Input(step => step.Configurations, d => d.Configurations)
                            .Output(d => d.PullSequence, step => new PullSequenceStepData { IndexModels = step.OutputIndexModels ?? new List<IIndexModel>() })
                            .ForEach(d => d.PullSequence.IndexModels)
                            .Do(ctx => ctx.StartWith<PullSingleStep>().Input(s => s.IndexModel, (d, fctx) => fctx.Item)).Delay(xx => TimeSpan.FromSeconds(1)));

            // Pull in parallel
            p = p.Do(then => then.StartWith<PullInParallelStep>()
                            .Input(step => step.Configurations, d => d.Configurations)
                            .Output(d => d.PullParallel, step => new PullParallelStepData { IndexModels = step.OutputIndexModels ?? new List<IIndexModel>() })
                            .ForEach(d => d.PullParallel.IndexModels)
                            .Do(ctx => ctx.StartWith<PullSingleStep>().Input(s => s.IndexModel, (d, fctx) => fctx.Item).Delay(xx => TimeSpan.FromSeconds(1))));
            p.Join();
        }

        private void OnQueue(IWorkflowBuilder<MainWorkflowData> builder)
        {
            builder
                .StartWith(x => { Console.WriteLine("Queue"); })
                .Delay(x => TimeSpan.FromSeconds(5));
        }

        private void OnReport(IWorkflowBuilder<MainWorkflowData> builder)
        {
            builder
                .StartWith(x => { Console.WriteLine("Report"); })
                .Delay(x => TimeSpan.FromSeconds(5));
        }

        private void OnPush(IWorkflowBuilder<MainWorkflowData> builder)
        {
            builder
                .StartWith(x => { Console.WriteLine("Push"); })
                .Delay(x => TimeSpan.FromSeconds(5));
        }
    }
}
