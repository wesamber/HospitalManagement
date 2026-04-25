using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Parient;

namespace HospitalManagement.Infrastructure.Persistence.Json;

//JsonSnapshotLogRepository<Patient, PatientJsonModel>, كمان لا تنسى هون هي وراثة 
public class JsonPatientRepository :  IPatientRepository
{
    public JsonPatientRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer)
        //: base(snapshotPath, logPath, fileStorage, serializer)
    {
    }

    public Task AddAsync(Patient entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Patient entity)
    {
        throw new NotImplementedException();
    }

    public Task<List<Patient>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Patient?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Patient? GetByNumber(int patientNumber)
    {
        throw new NotImplementedException();
    }

    //public Patient? GetByNumber(int patientNumber)
    //{
    //    var state = _cache ??
    //        throw new InvalidOperationException("Repository not initialized.");

    //    return state.FirstOrDefault(p => p.PatientNumber == patientNumber.ToString());
    //}

    public Task UpdateAsync(Patient entity)
    {
        throw new NotImplementedException();
    }
}
