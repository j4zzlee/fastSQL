using System;
using System.Collections.Generic;
using System.Text;
using FastSQL.Core;

namespace FastSQL.Sync.Core.Transformers
{
    public class StringTrimTransformerOptionManager : BaseOptionManager
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>();
        }
    }

    public class StringTrimTransformer : BaseTransformer
    {
        public StringTrimTransformer(StringTrimTransformerOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "XIzWcM66l0S7T5ugMrrswA==";

        public override string Name => "String Trim Transformer";

        public override string Description => "String Trim Transformer";
        
        public override object Transform(object value)
        {
            if (value == null)
            {
                return value;
            }

            var val = (string)value;
            return val?.Trim();
        }
    }
}
