using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Config;
using Microsoft.Data.SqlClient;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoSystemConfigRepository : AdoRepository<SystemConfig>, ISystemConfigRepository
{
    public AdoSystemConfigRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    public async Task<SystemConfig> GetAsync()
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        using var cmd = CreateCommand(
            "SELECT TOP 1 Id, BaseSalary FROM SystemConfig", conn);

        var config = await QuerySingleAsync(cmd, r =>
            new SystemConfig(baseSalary: GetDecimal(r, "BaseSalary")));

        return config ?? throw new InvalidOperationException(
            "SystemConfig not found — يجب أن يوجد صف واحد على الأقل في جدول SystemConfig.");
    }

    public async Task UpdateAsync(SystemConfig config)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        using var cmd = CreateCommand(
            "UPDATE SystemConfig SET BaseSalary = @BaseSalary WHERE Id = @Id",
            conn);

        AddGuidParam(cmd, "@Id", config.Id);
        AddParam(cmd, "@BaseSalary", config.BaseSalary);
        await cmd.ExecuteNonQueryAsync();
    }
    // مابدي ياهن 
    public Task<SystemConfig?> GetByIdAsync(Guid id) => throw new NotSupportedException();
    public Task<List<SystemConfig>> GetAllAsync() => throw new NotSupportedException();
    public Task AddAsync(SystemConfig entity) => throw new NotSupportedException();
    public Task DeleteAsync(SystemConfig entity) => throw new NotSupportedException();
}
