using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Settings
{
    public class EntitiesSettingsOptionManager : BaseOptionManager
    {
        public override void Dispose()
        {

        }
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>();
        }
    }
}
