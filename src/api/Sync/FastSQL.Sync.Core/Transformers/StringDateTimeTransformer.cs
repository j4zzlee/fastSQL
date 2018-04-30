using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FastSQL.Core;

namespace FastSQL.Sync.Core.Transformers
{
    public class StringDateTimeTransformerOptionManager : BaseOptionMananger
    {
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "transformer_source_format_qjYj66Rk4U2l3mhYwY13zQ==",
                    DisplayName = "Source Datetime Format",
                    Description = "Source Datetime Format. E.g: DD-MM-YY",
                    Value = string.Empty,
                    Example = "mm-dd-YYYY",
                    OptionGroupNames = new List<string>{ "Transformer" },
                },
                new OptionItem
                {
                    Name = "transformer_destination_format_7SOS6wwrLUuVzf4UstHp1Q==",
                    DisplayName = "Desitnation Datetime Format",
                    Description = "Desitnation Datetime Format. E.g: DD-MM-YY",
                    Value = string.Empty,
                    Example = "mm-dd-YYYY",
                    OptionGroupNames = new List<string>{ "Transformer" },
                }
            };
        }
    }

    public class StringDateTimeTransformer : BaseTransformer
    {
        public StringDateTimeTransformer(StringDateTimeTransformerOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "Lj4Q7tN+UEqDzn1KQ21btw==";

        public override string Name => "Datetime Transformer";

        public override string Description => "Transform DateTime from one format to another";

        public override object Transform(object value)
        {
            if (value == null)
            {
                return value;
            }

            var val = (string)value;
            var sourceFormat = Options.FirstOrDefault(o => o.Name == "source_format_qjYj66Rk4U2l3mhYwY13zQ==").Value;
            var destFormat = Options.FirstOrDefault(o => o.Name == "destination_format_7SOS6wwrLUuVzf4UstHp1Q==").Value;
            var dt = DateTime.ParseExact(val, sourceFormat, CultureInfo.InvariantCulture);
            return dt.ToString(destFormat);
        }
    }
}
