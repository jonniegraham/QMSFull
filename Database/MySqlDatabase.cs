using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Database
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDatabase
    {
        Task<bool> ExistsAsync(string tableName, IReadOnlyDictionary<string, dynamic> whereValues);

        Task<List<Dictionary<string, dynamic>>> SearchRowsAsync(string tableExpression, IReadOnlyList<string> requestedColumns,
            IReadOnlyList<string> searchColumns, string searchTerm);

        Task<List<Dictionary<string, dynamic>>> GetRowsAsync(string tableName,
            IReadOnlyDictionary<string, dynamic> whereValues);

        Task<int> InsertAsync(string tableName, IReadOnlyDictionary<string, dynamic> fieldValues);

        Task<int> UpdateAsync(string tableName, IReadOnlyDictionary<string, dynamic> setValues,
            IReadOnlyDictionary<string, dynamic> whereValues);
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class MySqlDatabase : IDatabase, IDisposable
    {
        private static MySql _mySqlInstance;
        private static object _lockObject;

        public MySqlDatabase()
        {
            _mySqlInstance = new MySql();
            _lockObject = new object();
        }

        [ComVisible(true)]
        public Task<bool> ExistsAsync(string tableName, IReadOnlyDictionary<string, dynamic> whereValues)
        {
            lock (_lockObject)
            {
                var query = $"SELECT * FROM {tableName}";
                AppendWhereClause(ref query, whereValues);

                try
                {
                    if (!_mySqlInstance.IsConnected()) throw new Exception("Cannot connect to MySQL database.");
                    using (var cmd = new MySqlCommand(query, _mySqlInstance.Connection))
                    {
                        AddParameters(cmd, whereValues);
                        return Task.FromResult(cmd.ExecuteReader().Read());
                    }
                }
                catch (MySqlException sqlException)
                {
                    return Task.FromException<bool>(ParseSqlException(sqlException));
                }
                catch (Exception exception)
                {
                    return Task.FromException<bool>(exception);
                }
            }
        }

        [ComVisible(true)]
        public Task<List<Dictionary<string, dynamic>>> SearchRowsAsync(string tableExpression, IReadOnlyList<string> requestedColumns, IReadOnlyList<string> searchColumns, string searchTerm)
        {
            lock (_lockObject)
            {
                var query = $"SELECT {ListColumnNames(requestedColumns)} FROM {tableExpression}";
                ListSearchDisjuncts(ref query, searchColumns, searchTerm);
                query += " LIMIT 20";
                var rows = new List<Dictionary<string, dynamic>>();
                try
                {
                    if (!_mySqlInstance.IsConnected()) throw new Exception("Cannot connect to MySQL database.");
                    using (var cmd = new MySqlCommand(query, _mySqlInstance.Connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, dynamic>();
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetName(i), reader.GetValue(i));
                                }
                                rows.Add(row);
                            }
                            return Task.FromResult(rows);
                        }
                    }
                }
                catch (MySqlException sqlException)
                {
                    return Task.FromException<List<Dictionary<string, dynamic>>>(ParseSqlException(sqlException));
                }
                catch (Exception exception)
                {
                    return Task.FromException<List<Dictionary<string, dynamic>>>(exception);
                }
            }
        }

        [ComVisible(true)]
        public Task<List<Dictionary<string, dynamic>>> GetRowsAsync(string tableName, IReadOnlyDictionary<string, dynamic> whereValues)
        {
            lock (_lockObject)
            {
                var query = $"SELECT * FROM {tableName}";
                AppendWhereClause(ref query, whereValues);

                var rows = new List<Dictionary<string, dynamic>>();
                try
                {
                    if (!_mySqlInstance.IsConnected()) throw new Exception("Cannot connect to MySQL database.");
                    using (var cmd = new MySqlCommand(query, _mySqlInstance.Connection))
                    {
                        AddParameters(cmd, whereValues);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, dynamic>();
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetName(i), reader.GetValue(i));
                                }
                                rows.Add(row);
                            }
                            return Task.FromResult(rows);
                        }
                    }
                }
                catch (MySqlException sqlException)
                {
                    return Task.FromException<List<Dictionary<string, dynamic>>>(ParseSqlException(sqlException));
                }
                catch (Exception exception)
                {
                    return Task.FromException<List<Dictionary<string, dynamic>>>(exception);
                }
            }
        }

        [ComVisible(true)]
        public Task<int> InsertAsync(string tableName, IReadOnlyDictionary<string, dynamic> fieldValues)
        {
            lock (_lockObject)
            {
                var query = $"INSERT INTO {tableName}({ListColumnNames(fieldValues)}) VALUES({ListValuePlaceholders(fieldValues)})";
                try
                {
                    if (!_mySqlInstance.IsConnected()) throw new Exception("Cannot connect to MySQL database.");
                    using (var cmd = new MySqlCommand(query, _mySqlInstance.Connection))
                    {
                        AddParameters(cmd, fieldValues);
                        cmd.ExecuteNonQuery();

                        return Task.FromResult((int)cmd.LastInsertedId);
                    }
                }
                catch (MySqlException sqlException)
                {
                    return Task.FromException<int>(ParseSqlException(sqlException));
                }
                catch (Exception exception)
                {
                    return Task.FromException<int>(exception);
                }
            }
        }

        [ComVisible(true)]
        public Task<int> UpdateAsync(string tableName, IReadOnlyDictionary<string, dynamic> setValues, IReadOnlyDictionary<string, dynamic> whereValues)
        {
            lock (_lockObject)
            {
                var query = $"UPDATE {tableName}";
                AppendSetStatement(ref query, setValues);
                AppendWhereClause(ref query, whereValues);

                try
                {
                    if (!_mySqlInstance.IsConnected()) throw new Exception("Cannot connect to MySQL database.");
                    using (var cmd = new MySqlCommand(query, _mySqlInstance.Connection))
                    {
                        AddParameters(cmd, setValues);
                        AddParameters(cmd, whereValues);
                        cmd.ExecuteNonQuery();

                        return Task.FromResult((int)cmd.LastInsertedId);
                    }
                }
                catch (MySqlException sqlException)
                {
                    return Task.FromException<int>(ParseSqlException(sqlException));
                }
                catch (Exception exception)
                {
                    return Task.FromException<int>(exception);
                }
            }
        }

        public void Dispose()
        {
            _mySqlInstance.Dispose();
        }
    }
}
