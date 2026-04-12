using HospitalManagement.Domain.Entities.Patients;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Patient? GetByNumber(int patientNumber);
}
