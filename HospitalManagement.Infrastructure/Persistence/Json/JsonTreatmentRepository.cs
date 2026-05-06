using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonTreatmentRepository : ITreatmentRepository
    //JsonSnapshotLogRepository<Treatment>, ITreatmentRepository
{
    public JsonTreatmentRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer) 
        //: base(snapshotPath, logPath, fileStorage, serializer)
    {
    }

    public Task AddAsync(Treatment entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Treatment entity)
    {
        throw new NotImplementedException();
    }

    public Task<List<Treatment>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<Treatment> GetByDate(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Treatment>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Treatment>> GetByDoctorAsync(Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Treatment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Treatment>> GetByIdsAsync(List<Guid> treatmentIds)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Treatment entity)
    {
        throw new NotImplementedException();
    }

    //public IReadOnlyList<Treatment> GetByDate(DateTime dateStart, DateTime dateEnd)
    //{
    //    var state = _cache ?? 
    //        throw new InvalidOperationException("Repository is not initialized.");
    //    return state.Where(t => t.StartDate >= dateStart && t.StartDate <= dateEnd).ToList();
    //}

    //public Task<IList<Treatment>> GetByIdsAsync(List<Guid> treatmentIds)
    //{
    //    var state = _cache ?? 
    //        throw new InvalidOperationException("Repository is not initialized.");
    //    var result = state.Where(t => treatmentIds.Contains(t.Id)).Cast<Treatment>().ToList();
    //    return Task.FromResult<IList<Treatment>>(result);
    //}

    //public Task<IList<Treatment>> GetByDoctorAsync(Guid doctorId)
    //{
    //    var state = _cache ?? 
    //        throw new InvalidOperationException("Repository is not initialized.");

    //    // For TreatmentInternal, check the Doctors collection
    //    var internalTreatments = state.OfType<TreatmentInternal>()
    //        .Where(t => t.Doctors.Any(d => d.DoctorId == doctorId))
    //        .Cast<Treatment>()
    //        .ToList();

    //    return Task.FromResult<IList<Treatment>>(internalTreatments);
    //}

    //public Task<IList<Treatment>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    //{
    //    var state = _cache ?? 
    //        throw new InvalidOperationException("Repository is not initialized.");

    //    // For TreatmentInternal, check the Doctors collection and date range
    //    var result = state.OfType<TreatmentInternal>()
    //        .Where(t => t.Doctors.Any(d => d.DoctorId == doctorId) && 
    //                    t.StartDate >= startDate && 
    //                    t.StartDate <= endDate)
    //        .Cast<Treatment>()
    //        .ToList();

    //    return Task.FromResult<IList<Treatment>>(result);
    //}
}
