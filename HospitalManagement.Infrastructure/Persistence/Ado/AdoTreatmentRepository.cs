using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoTreatmentRepository : AdoRepository<Treatment>, ITreatmentRepository
{
    public AdoTreatmentRepository(ISqlConnectionFactory sqlConnectionFactory)
        : base(sqlConnectionFactory)
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

    public Task<List<Treatment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<List<Treatment>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<List<Treatment>> GetByDoctorAsync(Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Treatment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Treatment>> GetByIdsAsync(List<Guid> treatmentIds)
    {
        throw new NotImplementedException();
    }

    public Task<Treatment?> GetByNumberAsync(string number)
    {
        throw new NotImplementedException();
    }

    public Task<List<Treatment>> GetByPatientIdAsync(Guid patientId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Treatment entity)
    {
        throw new NotImplementedException();
    }
}