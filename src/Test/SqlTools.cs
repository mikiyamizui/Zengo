using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Test
{
    public static class SqlTools
    {
        public static Action<string, IEnumerable<(string Name, object Value)>> SqlLogAction { get; set; }
        public static Action<string, object> SelectLogAction { get; set; }
        public static Action<string, object> InsertLogAction { get; set; }
        public static Action<string, object, object> UpdateLogAction { get; set; }
        public static Action<string, object> DropLogAction { get; set; }

        /// <summary>
        /// General SQL Command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IDbCommand GenerateSqlCommand(
            IDbConnection connection,
            string sql,
            IEnumerable<(string, object)> parameters = null
            )
        {
            var command = connection.CreateCommand();

            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException(nameof(sql));
            command.CommandText = sql;

            if (parameters != null && parameters.Any())
            {
                parameters.Distinct().ToList().ForEach(p =>
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = p.Item1;
                    parameter.Value = p.Item2;
                    command.Parameters.Add(parameter);
                });
            }

            SqlLogAction?.Invoke(command.CommandText,
                command.Parameters.Cast<IDbDataParameter>().Select(p => (p.ParameterName, p.Value)));

            return command;
        }

        /// <summary>
        /// Simple Select Command
        /// </summary>
        /// <typeparam name="TQueryObject"></typeparam>
        /// <typeparam name="TKeyObject"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="queryObject"></param>
        /// <param name="keyObject"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static IDbCommand GenerateSelectCommand<TQueryObject, TKeyObject>(
            IDbConnection connection,
            string tableName,
            TQueryObject queryObject,
            TKeyObject keyObject,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            )
        {
            var parameters = GenerateParameters(queryObject);
            var querySql = GetSelectInsertColumnsString(parameters);
            if (string.IsNullOrEmpty(querySql))
                querySql = "*";

            var keys = GenerateParameters(keyObject);
            var keySql = GetWhereConditionString(keys);

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException(nameof(tableName));

            var sql = $"select {querySql} from {tableName}{keySql}";

            SelectLogAction?.Invoke(sql, keyObject);

            return GenerateSqlCommand(connection, sql, parameters.Concat(keys).Distinct());
        }

        /// <summary>
        /// Insert Command
        /// </summary>
        /// <typeparam name="TParameterObject"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static IDbCommand GenerateInsertCommand<TParameterObject>(
            IDbConnection connection,
            string tableName,
            TParameterObject parameterObject,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            )
        {
            var parameters = GenerateParameters(parameterObject);
            if (!parameters.Any())
                throw new ArgumentException(nameof(parameterObject));

            var columnSql = GetSelectInsertColumnsString(parameters);
            var valueSql = GetInsertValuesString(parameters);

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException(nameof(tableName));
            var sql = $"insert into {tableName} ({columnSql}) values ({valueSql});";

            InsertLogAction?.Invoke(sql, parameterObject);

            return GenerateSqlCommand(connection, sql, parameters);
        }

        /// <summary>
        /// Update Command
        /// </summary>
        /// <typeparam name="TUpdateObject"></typeparam>
        /// <typeparam name="TKeyObject"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="updateObject"></param>
        /// <param name="keyObject"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static IDbCommand GenerateUpdateCommand<TUpdateObject, TKeyObject>(
            IDbConnection connection,
            string tableName,
            TUpdateObject updateObject,
            TKeyObject keyObject,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            )
        {
            var updates = GenerateParameters(updateObject);
            if (!updates.Any())
                throw new ArgumentException(nameof(updateObject));
            var updateSql = GetUpdateSetString(updates);

            var keys = GenerateParameters(keyObject);
            var keySql = GetWhereConditionString(keys);

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException(nameof(tableName));
            var sql = $"update {tableName} set {updateSql}{keySql};";

            UpdateLogAction?.Invoke(sql, updateObject, keyObject);

            return GenerateSqlCommand(connection, sql, updates.Concat(keys).Distinct());
        }

        /// <summary>
        /// Delete Command
        /// </summary>
        /// <typeparam name="TKeyObject"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="keyObject"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static IDbCommand GenerateDeleteCommand<TKeyObject>(
            IDbConnection connection,
            string tableName,
            TKeyObject keyObject,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            )
        {
            var keys = GenerateParameters(keyObject);
            var keySql = GetWhereConditionString(keys);

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException(nameof(tableName));
            var sql = $"delete from {tableName}{keySql};";

            DropLogAction?.Invoke(sql, keyObject);

            return GenerateSqlCommand(connection, sql, keys);
        }

        /// <summary>
        /// Convert object to IEnumerable<(string Name, object Value)>
        /// </summary>
        /// <typeparam name="TParameterObject"></typeparam>
        /// <param name="parameterObject"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        private static IEnumerable<(string Name, object Value)> GenerateParameters<TParameterObject>(
            TParameterObject parameterObject,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            )
            => typeof(TParameterObject).GetProperties(bindingFlags)
                .Select(pi => (pi.Name, pi.GetValue(parameterObject)));

        /// <summary>
        /// Convert IEnumerable<(string Name, object Value)> to "col1 = :col1, col2 = :col2" string
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static string GetUpdateSetString(IEnumerable<(string Name, object Value)> elements)
            => string.Join(", ", elements.Select(p => $"{p.Name} = :{p.Name}"));

        /// <summary>
        /// Convert IEnumerable<(string Name, object Value)> to " where col1 = :col1 and col2 = :col2" string
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static string GetWhereConditionString(IEnumerable<(string Name, object Value)> elements)
        {
            var sql = string.Join(" and ", elements.Select(p => $"{p.Name} = :{p.Name}"));

            if (!string.IsNullOrEmpty(sql))
            {
                sql = $" where {sql}";
            }

            return sql;
        }

        /// <summary>
        /// Convert IEnumerable<(string Name, object Value)> to "col1, col2" string
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static string GetSelectInsertColumnsString(IEnumerable<(string Name, object Value)> elements)
        {
            return string.Join(", ", elements.Select(elem => elem.Name));
        }

        /// <summary>
        /// Convert IEnumerable<(string Name, object Value)> to ":col1, :col2" string
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static string GetInsertValuesString(IEnumerable<(string Name, object Value)> elements)
        {
            return string.Join(", ", elements.Select(elem => $":{elem.Name}"));
        }
    }
}