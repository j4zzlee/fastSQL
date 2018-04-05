using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public class QueryResult
    {
        public string Id { get; set; }
        public int RecordsAffected { get; set; }
        public IEnumerable<object> Rows { get; set; }
    }
}
