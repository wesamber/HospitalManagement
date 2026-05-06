using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Parient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonPatientRepository : JsonSnapshotLogRepository<Patient, PatientJsonModel>, IPatientRepository
{
    public JsonPatientRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer)
        : base(
            snapshotPath,
            logPath,
            fileStorage,
            serializer,
            patient => patient.ToJsonModel(),
            model => model.ToDomain())
    { }

    public async Task<bool> ExistsAsync(string name, DateOnly? dateOfBirth)
    {
        var state = await GetAllAsync();
        return state.Any(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            p.DateOfBirth == dateOfBirth);
    }

    public async Task<IReadOnlyList<InternalPatient>> GetByDepartmentAsync(Guid departmentId)
    {
        var state = await GetAllAsync();
        return state
            .OfType<InternalPatient>()
            .Where(p => p.DepartmentId == departmentId)
            .ToList();
    }

    public async Task<Patient?> GetByNumberAsync(string patientNumber)
    {
        var state = await GetAllAsync();
        return state.FirstOrDefault(p =>
            p.PatientNumber.Equals(patientNumber, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<Patient>> GetByTypeAsync(string type)
    {
        var state = await GetAllAsync();
        return type.ToLower() == "internal"
            ? state.OfType<InternalPatient>().Cast<Patient>().ToList()
            : state.OfType<ExternalPatient>().Cast<Patient>().ToList();
    }

}
