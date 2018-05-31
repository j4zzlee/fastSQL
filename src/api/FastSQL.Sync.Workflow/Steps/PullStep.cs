using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class PullSingleStep : StepBody
    {
        private ILogger _logger;
        public IIndexModel IndexModel { get; set; }
        public int CurrentIndex { get; set; }

        public PullSingleStep(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _logger.Information($@"Processing indexer: {IndexModel.Name}");
            CurrentIndex++;
            return ExecutionResult.Next();
        }
    }

    public class PullInSequenceStep : StepBody
    {
        private ILogger _logger;
        public IEnumerable<object> Configurations { get; set; }
        public IEnumerable<IIndexModel> OutputIndexModels { get; set; }

        public PullInSequenceStep(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            OutputIndexModels = new List<EntityModel> {
                new EntityModel
                {
                    Name = "Entity 1",
                    Description = "Entity 1"
                },
                new EntityModel
                {
                    Name = "Entity 2",
                    Description = "Entity 2"
                },
                new EntityModel
                {
                    Name = "Entity 3",
                    Description = "Entity 3"
                }
            };
            return ExecutionResult.Next();
        }
    }

    public class PullInParallelStep : StepBody
    {
        private ILogger _logger;
        public IEnumerable<object> Configurations { get; set; }
        public IEnumerable<IIndexModel> OutputIndexModels { get; set; }

        public PullInParallelStep(ResolverFactory resolver)
        {
            _logger = resolver.Resolve<ILogger>("Workflow");
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            OutputIndexModels = new List<AttributeModel> {
                new AttributeModel
                {
                    Name = "Attribute 1",
                    Description = "Attribute 1"
                },
                new AttributeModel
                {
                    Name = "Attribute 2",
                    Description = "Attribute 2"
                },
                new AttributeModel
                {
                    Name = "Attribute 3",
                    Description = "Attribute 3"
                }
            };
            return ExecutionResult.Next();
        }
    }
}
