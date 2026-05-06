using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Treatments;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonTreatmentRepository : JsonSnapshotLogRepository<Treatment , TreatmentJsonModel>, ITreatmentRepository
{
    public JsonTreatmentRepository(
       string snapshotPath,
       string logPath,
       IFileStorage fileStorage,
       ISerializer serializer)
       : base(
           snapshotPath,
           logPath,
           fileStorage,
           serializer,
           t => t.ToJsonModel(),
           m => m.ToDomain())
    { }

    public async Task<List<Treatment>> GetByPatientIdAsync(Guid patientId)
    {
        var state = await GetAllAsync();
        return state.Where(t => t switch
        {
            TreatmentInternal i => i.PatientId == patientId,
            TreatmentExternal e => e.PatientId == patientId,
            _ => false
        }).ToList();
    }

    public async Task<List<Treatment>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var state = await GetAllAsync();
        return state.Where(t => ids.Contains(t.Id)).ToList();
    }

    public async Task<Treatment?> GetByNumberAsync(string number)
    {
        var state = await GetAllAsync();
        return state.FirstOrDefault(t =>
            t.NumberTreatment.Equals(number, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Treatment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var state = await GetAllAsync();
        return state.Where(t => t.StartDate >= startDate && t.StartDate <= endDate).ToList();
    }

    public async Task<List<Treatment>> GetByIdsAsync(List<Guid> treatmentIds)
    {
        var state = await GetAllAsync();
        return state.Where(t => treatmentIds.Contains(t.Id)).ToList();
    }

    public async Task<List<Treatment>> GetByDoctorAsync(Guid doctorId)
    {
        var state = await GetAllAsync();
        return state
            .OfType<TreatmentInternal>()
            .Where(t => t.Doctors.Any(d => d.DoctorId == doctorId))
            .Cast<Treatment>()
            .ToList();
    }

    public async Task<List<Treatment>> GetByDoctorAndPeriodAsync(
    Guid doctorId, DateTime startDate, DateTime endDate)
    {
        var state = await GetAllAsync();

        var internal_ = state
            .OfType<TreatmentInternal>()
            .Where(t =>
                t.Doctors.Any(d => d.DoctorId == doctorId) &&
                t.StartDate >= startDate &&
                t.StartDate <= endDate)
            .Cast<Treatment>();

        return internal_.ToList();
    }
}
