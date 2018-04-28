using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.UI.Interfaces
{
    public interface IControlDefinition
    {
        string Id { get; }
        string ControlName { get; }
        string ControlHeader { get; }
        string Description { get; }
        string ActivatedById { get; }
        int DefaultState { get; }
        object Control { get; }
    }
}
