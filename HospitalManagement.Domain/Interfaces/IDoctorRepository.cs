using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;

namespace HospitalManagement.Domain.Interfaces;

public interface IDoctorRepository : IRepository<Doctor>
{
    Doctor? GetByNumber(string doctorNumber);
    IReadOnlyList<Doctor> GetBySpecialization(enSpecialization specialization);
    IReadOnlyList<Doctor> GetByDepartment(Guid departmentId);
}
