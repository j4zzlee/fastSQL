using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using WorkflowCore.Interface;

namespace WorkflowCore.Extensions.WorkflowController
{
    public static class WorkflowControllerExtensions
    {
        public static void RegisterWorkflow(this IWorkflowHost host, IWorkflow workflow)
        {
            var reg = host.Registry;
            reg.RegisterWorkflow(workflow);
        }

        public static void StartGenericWorkflow(this IWorkflowHost host, object workflow)
        {
            //var reg = host.Registry;
            var method = host.GetType()
                .GetMethods()
                .Where(m => m.IsGenericMethod && m.Name == "StartWorkflow")
                .First();
            var workflowType = workflow.GetType();
            var buildMethod = workflowType
                .GetMethod("Build");
            var buildMethodParameterType = buildMethod.GetParameters()[0].ParameterType;
            var buildMethodDataType = buildMethodParameterType.GetGenericArguments()[0];
            var generic = method.MakeGenericMethod(buildMethodDataType);
            //var invokeType = workflowType
            //    .GetInterfaces()
            //    .FirstOrDefault(i => i.FullName == generic.GetParameters()[0].ParameterType.FullName);
            var dataInstance = Activator.CreateInstance(buildMethodDataType);
            var jWorkflow = JObject.FromObject(workflow);
            generic.Invoke(host, new[] { jWorkflow.Value<string>("Id"), dataInstance, "" });
        }

        public static void RegisterGenericWorkflow(this IWorkflowHost host, object workflow)
        {
            var reg = host.Registry;
            var method = reg.GetType()
                .GetMethods()
                .Where(m => m.IsGenericMethod && m.Name == "RegisterWorkflow")
                .First();
            var workflowType = workflow.GetType();
            var buildMethod = workflowType
                .GetMethod("Build");
            var buildMethodParameterType = buildMethod.GetParameters()[0].ParameterType;
            var buildMethodDataType = buildMethodParameterType.GetGenericArguments()[0];
            var generic = method.MakeGenericMethod(buildMethodDataType);
            var invokeType = workflowType
                .GetInterfaces()
                .FirstOrDefault(i => i.FullName == generic.GetParameters()[0].ParameterType.FullName);
            generic.Invoke(reg, new[] { workflow });
        }
    }
}
