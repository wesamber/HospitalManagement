using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonTreatmentRepository : JsonSnapshotLogRepository<Treatment>, ITreatmentRepository
{
    public JsonTreatmentRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer) 
        : base(snapshotPath, logPath, fileStorage, serializer)
    {
    }

    public IReadOnlyList<Treatment> GetByDate(DateTime dateStart, DateTime dateEnd)
    {
        var state = _cache ?? 
            throw new InvalidOperationException("Repository is not initialized.");
        return state.Where(t => t.StartDate >= dateStart && t.StartDate <= dateEnd).ToList();
    }
}
