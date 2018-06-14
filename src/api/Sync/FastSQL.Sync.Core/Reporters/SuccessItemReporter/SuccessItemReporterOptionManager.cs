using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Reporters
{
    public class SuccessItemReporterOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "show_debug_info",
                    DisplayName = "Show Debug Info",
                    Description = @"Show Debug Info",
                    Value = bool.FalseString,
                    Type = OptionType.Boolean,
                    OptionGroupNames = new List<string>{ "SuccessItemReporterOptionManager" },
                },
                new OptionItem
                {
                    Name = "show_progress_info",
                    DisplayName = "Show Progress Info",
                    Description = @"Shows information about sync progress",
                    Value = bool.FalseString,
                    Type = OptionType.Boolean,
                    OptionGroupNames = new List<string>{ "SuccessItemReporterOptionManager" },
                },
            };
        }
    }
}
