using Db.Core.Abstractions.Sql.Interfaaces;
using MySqlConnector;
using RnzTrauer.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime;

namespace Db.Provider.MySql
{
    /// <summary>
    /// Creates MySQL-specific ADO.NET objects for the neutral DB framework.
    /// </summary>
    public sealed class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection(IDBSettings xSettings)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(xSettings);
#else
            if (xSettings == null) throw new ArgumentNullException(nameof(xSettings));
#endif

            var sConnectionString = new MySqlConnectionStringBuilder
            {
                Server = xSettings[nameof(MySqlConnectionStringBuilder.Server)].ToString(),
                Port = (uint?)xSettings[nameof(MySqlConnectionStringBuilder.Port)] ?? 3306,
                UserID = xSettings[nameof(MySqlConnectionStringBuilder.UserID)].ToString(),
                Password = xSettings[nameof(MySqlConnectionStringBuilder.Password)]?.ToString() ?? "",
                Database = xSettings[nameof(MySqlConnectionStringBuilder.Database)].ToString(),
                AllowUserVariables = true,
                ConvertZeroDateTime = true
            }.ConnectionString;

            return new MySqlConnection(sConnectionString);
        }

        public IDBSettings CreateSettingsStub()
        {
            return new MySqlDbSettings
            {
                [nameof(MySqlConnectionStringBuilder.Server)] = string.Empty,
                [nameof(MySqlConnectionStringBuilder.Port)] = 3306u,
                [nameof(MySqlConnectionStringBuilder.UserID)] = string.Empty,
                [nameof(MySqlConnectionStringBuilder.Password)] = string.Empty,
                [nameof(MySqlConnectionStringBuilder.Database)] = string.Empty
            };
        }


        /// <summary>
        /// Lightweight settings wrapper so dictionary values can be returned as <see cref="IDBSettings"/>.
        /// </summary>
        private sealed class MySqlDbSettings : Dictionary<string, object>, IDBSettings
        {
        }

        /// <inheritdoc />
        public IDbStatementRenderer CreateStatementRenderer(IDbConnection dBConnection) 
            => new MySqlStatementRenderer(dBConnection);


    }
}
