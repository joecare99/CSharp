using System.Data.Common;
using System.Data.OleDb;

namespace DBTest1
{
    public sealed class OleDbConnectorFactory : DbProviderFactory
    {
        public static readonly OleDbConnectorFactory Instance = new OleDbConnectorFactory();
        private OleDbConnectorFactory() { }

        public override DbCommand? CreateCommand() => new OleDbCommand();
        public override DbCommandBuilder? CreateCommandBuilder() => new OleDbCommandBuilder();
        public override DbConnection? CreateConnection() => new OleDbConnection();
        public override DbConnectionStringBuilder? CreateConnectionStringBuilder() => new OleDbConnectionStringBuilder();
        public override DbDataAdapter? CreateDataAdapter() => new OleDbDataAdapter();
        public override DbParameter? CreateParameter() => new OleDbParameter();

    }
}