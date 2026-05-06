using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Departments;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Departments;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonDepartmentRepository : JsonSnapshotLogRepository<Department, DepartmentJsonModel>, IDepartmentRepository
{
    public JsonDepartmentRepository(
        string snapshotPath,
        string logPath, 
        IFileStorage fileStorage , 
        ISerializer serializer) : base(snapshotPath,
            logPath,
            fileStorage,
            serializer,
            department => department.ToJsonModel(),
            model => model.ToDomain())
    { }
    public async Task<bool> ExistsAsync(string name)
    {
        var state = await GetAllAsync();
        return state.Any(d =>
            d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        var state = await GetAllAsync();
        return state.FirstOrDefault(d =>
            d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
