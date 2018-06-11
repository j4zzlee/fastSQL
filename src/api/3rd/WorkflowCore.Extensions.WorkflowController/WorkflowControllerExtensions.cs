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
            //var castMethod = workflowType.GetMethod("Cast",
            //    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            //var castM = typeof(BaseWorkflow<>).GetMethods();

            // Binding the method info to generic arguments               
            //Type[] genericArguments = new Type[] { invokeType };
            //MethodInfo genericMethodInfo = castMethod.MakeGenericMethod(genericArguments);

            // Simply invoking the method and passing parameters  
            // The null parameter is the object to call the method from. Since the method is  
            // static, pass null.  

            generic.Invoke(reg, new[] { workflow });
        }
    }
}
