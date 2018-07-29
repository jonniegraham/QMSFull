using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Database
{
    public partial class MySqlDatabase
    {
        private readonly IReadOnlyDictionary<Type, MySqlDbType> _dbTypes = new Dictionary<Type, MySqlDbType>()
        {
            {typeof(string), MySqlDbType.VarChar },
            {typeof(int), MySqlDbType.Int32 },
            {typeof(decimal), MySqlDbType.Decimal },
            {typeof(DateTime), MySqlDbType.DateTime }
        };

        private static void AppendWhereClause(ref string query, IReadOnlyDictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                return;

            query += " WHERE " + columnValuePairs.First().Key + "=@" + columnValuePairs.First().Key;
            query = columnValuePairs.Skip(1).Aggregate(query, (current, columnValuePair) => current + (" AND " + columnValuePair.Key + "=@" + columnValuePair.Key));
        }

        private static void AppendSetStatement(ref string query, IReadOnlyDictionary<string, object> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                throw new ArgumentException("columnValuePairs cannot be null or empty.");

            query += " SET " + columnValuePairs.First().Key + "=@" + columnValuePairs.First().Key;
            query = columnValuePairs.Skip(1).Aggregate(query, (current, columnValuePair) => current + (", " + columnValuePair.Key + "=@" + columnValuePair.Key));
        }

        private static void ListSearchDisjuncts(ref string query, IReadOnlyList<string> searchColumns, string searchTerm)
        {
            query += $" WHERE LOCATE('{searchTerm}', {searchColumns.First()})";
            for (var i = 1; i < searchColumns.Count; i++)
            {
                query += $" OR LOCATE('{searchTerm}',{searchColumns[i]})";
            }
            query += " > 0";
        }

        private void AddParameters(MySqlCommand command, IReadOnlyDictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null)
                return;

            foreach (var columnValuePair in columnValuePairs)
            {
                command.Parameters.Add($"@{columnValuePair.Key}", GetDbType(columnValuePair.Value));
                command.Parameters[$"@{columnValuePair.Key}"].Value = columnValuePair.Value;
            }
        }

        private MySqlDbType GetDbType(dynamic value)
        {
            Type type = value.GetType();
            if (!_dbTypes.ContainsKey(type))
                throw new ArgumentException($"C# type '{type}' is not mapped to any SqlDbType.");
            return _dbTypes[type];
        }

        private static string ListColumnNames(IReadOnlyDictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                throw new ArgumentException("columnValuePairs cannot be null or empty.");

            var listedColumnNames = "";
            listedColumnNames += columnValuePairs.First().Key;
            return columnValuePairs.Skip(1).Aggregate(listedColumnNames, (current, columnValuePair) => current + (", " + columnValuePair.Key));
        }

        private static string ListColumnNames(IReadOnlyCollection<string> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                throw new ArgumentException("columnValuePairs cannot be null or empty.");

            var listedColumnNames = "";
            listedColumnNames += columnValuePairs.First();
            return columnValuePairs.Skip(1).Aggregate(listedColumnNames, (current, columnValuePair) => current + ", " + columnValuePair);
        }

        private static string ListValuePlaceholders(IReadOnlyDictionary<string, dynamic> columnValuePairs)
        {
            return "@" + Utilities.Replace(@",\s*", ", @", ListColumnNames(columnValuePairs));
        }

        private static Exception ParseSqlException(MySqlException sqlException)
        {
            if (sqlException.Number == 134)
                return new ArgumentException("(SQL Exception #134) Cannot update a field that is used in the WHERE clause.");
            if (sqlException.Number == 207)
                return new ArgumentException("SQL Exception #207: Invalid column table name.");
            if (sqlException.Number == 208)
                return new ArgumentException("SQL Exception #208: Invalid database table name.");
            return sqlException;
        }
    }
}