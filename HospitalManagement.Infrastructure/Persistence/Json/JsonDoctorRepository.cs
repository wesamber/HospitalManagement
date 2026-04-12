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
    {
    }

    public IReadOnlyList<Doctor> GetByDepartment(Guid departmentId)
    {
        var state = _cache ?? 
            throw new InvalidOperationException("Repository is not initialized.");
        return state.Where(d => d.DepartmentsIds.Contains(departmentId)).ToList();   
    }


    public Doctor? GetByNumber(string doctorNumber)
    {
        var state = _cache ?? throw new InvalidOperationException("Repository not initialized.");
        return state.FirstOrDefault(d => d.DoctorNumber == doctorNumber);
    }


    public IReadOnlyList<Doctor> GetBySpecialization(Specialization specialization)
    {
        var state = _cache ?? throw new InvalidOperationException("Repository not initialized.");
        return state.Where(d => d.Specialization == specialization).ToList();
    }
}
