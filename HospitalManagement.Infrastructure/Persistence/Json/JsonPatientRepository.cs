using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Patients;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonPatientRepository : JsonSnapshotLogRepository<Patient>, IPatientRepository
{
    public JsonPatientRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer) 
        : base(snapshotPath, logPath, fileStorage, serializer)
    {
    }

    public Patient? GetByNumber(int patientNumber)
    {
        var state = _cache ?? 
            throw  new InvalidOperationException("Repository not initialized.");

        return state.FirstOrDefault(p => p.PatientNumber == patientNumber.ToString());
    }
}
