using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DbContextExtensions
    {
        public static async Task<List<T>> RawSqlQuery<T>(this DbContext dbContext, string query, Func<DbDataReader, T> map)
        {
            using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            dbContext.Database.OpenConnection();

            using var result = await command.ExecuteReaderAsync();
            var entities = new List<T>();

            while (result.Read())
            {
                entities.Add(map(result));
            }

            return entities;
        }
    }
}