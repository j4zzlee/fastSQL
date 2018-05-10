using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.UI.Interfaces
{
    public interface IControlDefinition
    {
        string Id { get; set; }
        string ControlName { get; set; }
        string ControlHeader { get; set; }
        string Description { get; set; }
        string ActivatedById { get; set; }
        int DefaultState { get; }
        object Control { get; }
    }
}
