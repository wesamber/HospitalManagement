using HospitalManagement.Domain.Entities.Patients;

namespace HospitalManagement.Domain.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Patient GetByNumber(int patientNumber);
}
