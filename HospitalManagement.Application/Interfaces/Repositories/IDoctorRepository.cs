using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Doctor? GetByNumber(string doctorNumber);
    IReadOnlyList<Doctor> GetBySpecialization(Specialization specialization);
    IReadOnlyList<Doctor> GetByDepartment(Guid departmentId);
}
