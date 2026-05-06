using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface ITreatmentRepository : IRepository<Treatment>
{
    Task<List<Treatment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Treatment>> GetByIdsAsync(List<Guid> treatmentIds);
    Task<List<Treatment>> GetByDoctorAsync(Guid doctorId);
    Task<List<Treatment>> GetByPatientIdAsync(Guid patientId);
    Task<Treatment?> GetByNumberAsync(string number);
    Task<List<Treatment>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate);
}
