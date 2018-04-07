using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;

namespace FastSQL.Core
{
    public abstract class BaseAdapter : IRichAdapter
    {
        protected IRichProvider Provider;
        protected IEnumerable<OptionItem> Options;
        protected BaseAdapter(IRichProvider provider)
        {
            Provider = provider;
        }

        public virtual IRichProvider GetProvider()
        {
            return Provider;
        }

        public virtual IRichAdapter SetOptions(IEnumerable<OptionItem> options)
        {
            Options = options;
            return this;
        }

        protected abstract DbConnection GetConnection();

        public abstract bool TryConnect(out string message);

        public abstract IEnumerable<string> GetTables();

        public abstract IEnumerable<string> GetViews();

        public virtual IEnumerable<QueryResult> Query(string raw, object @params = null)
        {
            IDbConnection conn = null;
            try
            {
                using (conn = GetConnection())
                {
                    conn.Open();
                    var reader = conn.ExecuteReader(raw, @params);
                    var results = new List<QueryResult>();
                    using (var sw = new StringWriter())
                    using (var writer = new JsonTextWriter(sw))
                    {
                        writer.WriteStartArray();
                        do
                        {
                            var row = 0;
                            var hasRow = false;
                            writer.WriteStartObject();
                            while (reader.Read())
                            {
                                if (row++ == 0)
                                {
                                    writer.WritePropertyName(nameof(QueryResult.Id));
                                    writer.WriteValue(Guid.NewGuid());
                                    writer.WritePropertyName(nameof(QueryResult.RecordsAffected));
                                    writer.WriteValue(reader.RecordsAffected);
                                    writer.WritePropertyName(nameof(QueryResult.Rows));
                                    writer.WriteStartArray();
                                    hasRow = true;
                                }
                                writer.WriteStartObject();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (!reader.IsDBNull(i))
                                    {
                                        writer.WritePropertyName(reader.GetName(i));
                                        writer.WriteValue(reader.GetValue(i));
                                    }
                                }
                                writer.WriteEndObject();
                            }
                            if (hasRow)
                            {
                                writer.WriteEndArray();
                            }
                            else
                            {
                                writer.WritePropertyName(nameof(QueryResult.RecordsAffected));
                                writer.WriteValue(reader.RecordsAffected);
                            }

                            writer.WriteEndObject();
                        } while (reader.NextResult()); // another table
                        writer.WriteEndArray();
                        results = JsonConvert.DeserializeObject<List<QueryResult>>(sw.ToString());
                    }

                    return results;
                }
            }
            finally
            {
                conn?.Dispose();
            }
        }

        public virtual int Execute(string raw, object @params = null)
        {
            IDbConnection conn = null;
            try
            {
                using (conn = GetConnection())
                {
                    conn.Open();
                    return conn.Execute(raw, @params);
                }
            }
            finally
            {
                conn?.Dispose();
            }
        }

        
    }
}
