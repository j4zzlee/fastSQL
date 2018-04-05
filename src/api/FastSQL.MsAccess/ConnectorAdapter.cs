using FastSQL.Core;
using System.Data;
using System.Data.Odbc;

namespace FastSQL.MsAccess
{
    public class ConnectorAdapter : BaseAdapter
    {
        private string _filePath = string.Empty;

        public override IDbConnection GetConnection()
        {
            return new OdbcConnection($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={_filePath};");
        }

        internal ConnectorAdapter SetFilePath(string filePath)
        {
            _filePath = filePath;
            return this;
        }
    }
}
