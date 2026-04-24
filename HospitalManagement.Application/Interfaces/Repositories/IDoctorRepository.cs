using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<Doctor?> GetByNumberAsync(string doctorNumber);
    Task<IReadOnlyList<Doctor>> GetBySpecializationAsync(Specialization specialization);
    Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId);
    Task<bool> ExistAsync(string name, DateOnly dob);
}
