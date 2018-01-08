// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.CodeTools.Sql
{
    public abstract class SqlQuery
    {

        protected bool _initialized;
        protected SqlCommand _command;

        protected abstract string GetQueryString();

        protected virtual async Task ExecuteNonQuery(SqlConnection connection, string query, params SqlParameter[] paramteres)
        {
            var command = new SqlCommand(query);
            InitializeCommand(command, connection, paramteres);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        protected virtual async Task ExecuteNonQuery(SqlConnection connection, params SqlParameter[] paramteres)
        {
            if (!_initialized)
                Initialize();

            InitializeCommand(_command, connection, paramteres);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();
            await _command.ExecuteNonQueryAsync();
        }

        protected void InitializeCommand(SqlCommand command, SqlConnection connection, SqlParameter[] paramteres)
        {
            command.Connection = connection;
            if (paramteres != null)
            {
                command.Parameters.Clear();
                command.Parameters.AddRange(paramteres);
            }
        }

        protected virtual void Initialize()
        {
            _command = new SqlCommand(GetQueryString());
            _initialized = true;
        }
    }

    public abstract class SqlQuery<TResult> : SqlQuery where TResult : class, new()
    {

        private IEnumerable<PropertyInfo> _propInfo;

        protected virtual async Task<IEnumerable<TResult>> Execute(SqlConnection connection, params SqlParameter[] paramteres)
        {
            if (!_initialized)
                Initialize();

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            InitializeCommand(_command, connection, paramteres);
            var reader = await _command.ExecuteReaderAsync();
            var res = new List<TResult>();
            while (await reader.ReadAsync())
            {
                res.Add(MapToResult(reader));
            }
            reader.Dispose();
            return res;
        }

        protected virtual TResult MapToResult(SqlDataReader reader)
        {
            if (_propInfo == null)
            {
                _propInfo = new List<PropertyInfo>(typeof(TResult).GetProperties().Where(e => e.SetMethod != null && !e.IsDefined(typeof(SqlMapIgnoreAttribute))));
            }
            var res = new TResult();
            foreach (var propertyInfo in _propInfo)
            {
                propertyInfo.SetValue(res, reader[propertyInfo.Name]);
            }
            return res;
        }

    }
}
