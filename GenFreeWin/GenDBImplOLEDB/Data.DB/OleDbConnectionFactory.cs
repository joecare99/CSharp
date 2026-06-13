using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Db.Core.Abstractions.Sql.Interfaaces;

namespace GenFree.Data.DB
{
    /// <summary>
    /// Creates OleDb-specific ADO.NET objects for the neutral DB framework.
    /// </summary>
    public sealed class OleDbConnectionFactory : IDbConnectionFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The provider connection string to apply to created connections.</param>
        public OleDbConnectionFactory()
        {
        }

        /// <inheritdoc />
        public IDbConnection CreateConnection(IDBSettings xSettings)
        {

           var builder = new OleDbConnectionStringBuilder()
            {

                Provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.16.0" : "Microsoft.Jet.OLEDB.4.0",
                DataSource = xSettings[nameof(OleDbConnectionStringBuilder.DataSource)].ToString(),
                PersistSecurityInfo = false
            };
            return new OleDbConnection(builder.ConnectionString);
        }

        public DbConnection CreateConnection()
        {
            throw new NotImplementedException();
        }

        public IDBSettings CreateSettingsStub()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IDbStatementRenderer CreateStatementRenderer(IDbConnection dBConnection) 
            => new OleDbStatementRenderer(dBConnection);
    }
}
