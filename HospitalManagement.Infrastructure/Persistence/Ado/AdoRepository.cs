using HospitalManagement.Application.Interfaces.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public abstract class AdoRepository<T> where T : class
{
    protected readonly ISqlConnectionFactory _connectionFactory;

    protected AdoRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    protected async Task ExecuteInTransactionAsync(
        Func<SqlConnection, SqlTransaction, Task> work)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();
        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync();
        try
        {
            await work(conn, tx);
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    protected static SqlCommand CreateCommand(
        string sql,
        SqlConnection conn,
        SqlTransaction? tx = null)
    {
        var cmd = new SqlCommand(sql, conn);
        if (tx != null) cmd.Transaction = tx;
        return cmd;
    }

    protected static async Task<List<TResult>> QueryAsync<TResult>(
        SqlCommand cmd,
        Func<SqlDataReader, TResult> map)
    {
        var results = new List<TResult>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            results.Add(map(reader));
        return results;
    }

    protected static async Task<TResult?> QuerySingleAsync<TResult>(
        SqlCommand cmd,
        Func<SqlDataReader, TResult> map) where TResult : class
    {
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return map(reader);
        return null;
    }

    protected static Guid GetGuid(SqlDataReader r, string col)
        => r.GetGuid(r.GetOrdinal(col));

    protected static string GetString(SqlDataReader r, string col)
        => r.GetString(r.GetOrdinal(col));

    protected static int GetInt(SqlDataReader r, string col)
        => r.GetInt32(r.GetOrdinal(col));

    protected static decimal GetDecimal(SqlDataReader r, string col)
        => r.GetDecimal(r.GetOrdinal(col));

    protected static DateTime GetDateTime(SqlDataReader r, string col)
        => r.GetDateTime(r.GetOrdinal(col));

    protected static DateOnly GetDateOnly(SqlDataReader r, string col)
        => DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal(col)));

    protected static bool GetBool(SqlDataReader r, string col)
        => r.GetBoolean(r.GetOrdinal(col));

    protected static Guid? GetNullableGuid(SqlDataReader r, string col)
    {
        var ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetGuid(ord);
    }

    protected static string? GetNullableString(SqlDataReader r, string col)
    {
        var ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetString(ord);
    }

    protected static DateTime? GetNullableDateTime(SqlDataReader r, string col)
    {
        var ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetDateTime(ord);
    }


    protected static decimal? GetNullableDecimal(SqlDataReader r, string col)
    {
        var ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetDecimal(ord);
    }

    protected static T? GetNullableEnum<T>(SqlDataReader r, string col)
        where T : struct, Enum
    {
        var ord = r.GetOrdinal(col);
        if (r.IsDBNull(ord)) return null;
        return Enum.Parse<T>(r.GetString(ord), ignoreCase: true);
    }

    protected static T GetEnum<T>(SqlDataReader r, string col)
        where T : struct, Enum
        => Enum.Parse<T>(r.GetString(r.GetOrdinal(col)), ignoreCase: true);

    protected static void AddParam(SqlCommand cmd, string name, object? value)
       => cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

    protected static void AddGuidParam(SqlCommand cmd, string name, Guid value)
        => cmd.Parameters.Add(new SqlParameter(name, SqlDbType.UniqueIdentifier) { Value = value });

    protected static void AddNullableGuidParam(SqlCommand cmd, string name, Guid? value)
        => cmd.Parameters.Add(new SqlParameter(name, SqlDbType.UniqueIdentifier)
        { Value = value.HasValue ? value.Value : DBNull.Value });

    protected static void AddNullableDateParam(SqlCommand cmd, string name, DateTime? value)
        => cmd.Parameters.Add(new SqlParameter(name, SqlDbType.DateTime2)
        { Value = value.HasValue ? value.Value : DBNull.Value });
}
