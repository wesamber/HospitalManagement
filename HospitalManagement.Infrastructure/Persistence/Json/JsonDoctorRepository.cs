using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonDoctorRepository : JsonSnapshotLogRepository<Doctor> , IDoctorRepository
{
    public JsonDoctorRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer)
        : base(snapshotPath, logPath, fileStorage, serializer)
    {   }

    public async Task<bool> ExistAsync(string name, DateOnly dob)
    {
        var state = await GetAllAsync();
        return state.Any(d => d.Name == name && d.DateOfBirth == dob);
    }

    public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId)
    {
        var state = await GetAllAsync();
        return state
            .Where(d => d.DepartmentsIds.Contains(departmentId))
            .ToList();   
    }

    public async Task<Doctor?> GetByNumberAsync(string doctorNumber)
    {
        var state = await GetAllAsync();
        return state.FirstOrDefault(d => d.DoctorNumber == doctorNumber);
    }


    public async Task<IReadOnlyList<Doctor>> GetBySpecializationAsync(Specialization specialization)
    {
        var state = await GetAllAsync();
        return state
            .Where(d => d.Specialization == specialization)
            .ToList();
    }

     
}
