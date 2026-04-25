using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Config;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;
// مافي داعة لل log وال snapshot 
public class JsonSystemConfigRepository : ISystemConfigRepository
{
    private readonly string _filePath;
    private readonly IFileStorage _fileStorage;
    private readonly ISerializer _serializer;

    private static readonly Guid FixedId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly decimal DefaultBaseSalary = 1000m;

    public JsonSystemConfigRepository(
        string filePath,
        IFileStorage fileStorage,
        ISerializer serializer)
    {
        _filePath = filePath;
        _fileStorage = fileStorage;
        _serializer = serializer;
    }

    public async Task<SystemConfig> GetAsync()
    {
        if (!File.Exists(_filePath))
            return new SystemConfig(DefaultBaseSalary); // default

        var content = await _fileStorage.ReadAsync(_filePath);

        if (string.IsNullOrWhiteSpace(content))
            return new SystemConfig(DefaultBaseSalary);

        var model = _serializer.Deserialize<SystemConfigJsonModel>(content);
        return model?.ToDomain() ?? new SystemConfig(DefaultBaseSalary);
    }

    public async Task UpdateAsync(SystemConfig config)
    {
        var model = config.ToJsonModel();
        var json = _serializer.Serialize(model, indented: true);
        await _fileStorage.WriteAsync(_filePath, json);
    }
}
