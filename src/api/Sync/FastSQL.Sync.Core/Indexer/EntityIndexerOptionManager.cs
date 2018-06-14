using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Indexer
{
    public class EntityIndexerOptionManager : BaseOptionManager
    {
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
                //new OptionItem
                //{
                //    Name = "indexer_primary_key_columns",
                //    DisplayName = "@Primary Key Columns",
                //    Description = @"A comma separated list of columns that could be use to check if the item is exists or get destination id",
                //    Value = string.Empty,
                //    Example = "id:int",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
                //new OptionItem
                //{
                //    Name = "indexer_value_columns",
                //    DisplayName = "@Value Columns",
                //    Description = "A comma separated list of columns that are marked as [KEYS] and useful for system to track for changes.",
                //    Value = string.Empty,
                //    Example = "sku:nvarchar(max),name:text",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
                //new OptionItem
                //{
                //    Name = "indexer_report_alias_name",
                //    DisplayName = "@Report Alias Column",
                //    Description = "A comma separated list of columns alias name which might be useful when reporting.",
                //    Value = string.Empty,
                //    Example = "Sku",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
                //new OptionItem
                //{
                //    Name = "indexer_report_main_column_name",
                //    DisplayName = "@Report Main Column",
                //    Description = "A comma separated list of columns which aliased by [@Report Alias Column].",
                //    Value = string.Empty,
                //    Example = "Sku",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
                //new OptionItem
                //{
                //    Name = "indexer_alias_name",
                //    DisplayName = "@Alias Column",
                //    Description = "A comma separated list of columns alias name which might be useful when export to CSV, Excel files.",
                //    Value = string.Empty,
                //    Example = "Sku",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
                //new OptionItem
                //{
                //    Name = "indexer_main_column_name",
                //    DisplayName = "@Main Column",
                //    Description = "A comma separated list of columns which aliased by [@Alias Column].",
                //    Value = string.Empty,
                //    Example = "Sku",
                //    OptionGroupNames = new List<string>{ "Indexer" },
                //},
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
