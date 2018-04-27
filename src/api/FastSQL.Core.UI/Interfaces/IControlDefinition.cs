﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.UI.Interfaces
{
    public interface IControlDefinition
    {
        string Id { get; }
        string ControlName { get; }
        string Description { get; }
        bool IsActive { get; set; }
        string ActivatedById { get; }
        int DefaultState { get; }
        int DefaultPosition { get; }
        object Control { get; }
    }
}
