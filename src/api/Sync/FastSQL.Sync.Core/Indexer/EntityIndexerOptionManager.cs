using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class EntityIndexerOptionManager : BaseOptionManager
    {
        public override void Dispose()
        {

        }
        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>
            {
                new OptionItem
                {
                    Name = "indexer_mapping_columns",
                    DisplayName = "@Columns Mapping",
                    Description = @"List of columns mapping",
                    Value = string.Empty,
                    Type = OptionType.Grid,
                    SourceType = typeof(IndexColumnMapping),
                    OptionGroupNames = new List<string>{ "Indexer" },
                },
                new OptionItem
                {
                    Name = "indexer_reporter_columns",
                    DisplayName = "@Reporting Columns Mapping",
                    Description = @"List of columns mapping",
                    Value = string.Empty,
                    Type = OptionType.Grid,
                    SourceType = typeof(ReporterColumnMapping),
                    OptionGroupNames = new List<string>{ "Indexer" },
                },
                new OptionItem
                {
                    Name = "indexer_export_columns",
                    DisplayName = "@Export Columns Mapping",
                    Description = @"List of columns mapping",
                    Value = string.Empty,
                    Type = OptionType.Grid,
                    SourceType = typeof(CSVColumnMapping),
                    OptionGroupNames = new List<string>{ "Indexer" },
                },
                new OptionItem
                {
                    Name = "indexer_column_spliter",
                    DisplayName = "@Column Spliter",
                    Description = "A spliter character between columns which might be useful when export to CSV, Excel file.",
                    Value = "|",
                    Example = "|",
                    OptionGroupNames = new List<string>{ "Indexer" },
                }
            };
        }
    }
}
