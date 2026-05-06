using HospitalManagement.Domain.Entities.Patients;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByNumberAsync(string patientNumber);
    Task<bool> ExistsAsync(string name, DateOnly? dateOfBirth);
    Task<IReadOnlyList<InternalPatient>> GetByDepartmentAsync(Guid departmentId);
    Task<IReadOnlyList<Patient>> GetByTypeAsync(string type); // internal - external
}

