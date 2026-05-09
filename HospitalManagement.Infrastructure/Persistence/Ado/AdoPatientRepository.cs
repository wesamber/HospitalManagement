using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoPatientRepository : AdoRepository<Patient>, IPatientRepository
{
    public AdoPatientRepository(ISqlConnectionFactory sqlConnectionFactory) 
        : base(sqlConnectionFactory)
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

    public Task<bool> ExistsAsync(string name, DateOnly? dateOfBirth)
    {
        throw new NotImplementedException();
    }

    public Task<List<Patient>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<InternalPatient>> GetByDepartmentAsync(Guid departmentId)
    {
        throw new NotImplementedException();
    }

    public Task<Patient?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Patient?> GetByNumberAsync(string patientNumber)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Patient>> GetByTypeAsync(string type)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Patient entity)
    {
        throw new NotImplementedException();
    }
}
