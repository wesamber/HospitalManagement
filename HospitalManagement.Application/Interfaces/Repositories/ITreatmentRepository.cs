using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface ITreatmentRepository : IRepository<Treatment>
{
    IReadOnlyList<Treatment> GetByDate(DateTime dateStart , DateTime dateEnd);
    Task<IList<Treatment>> GetByIdsAsync(List<Guid> treatmentIds);
    Task<IList<Treatment>> GetByDoctorAsync(Guid doctorId);
    Task<IList<Treatment>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate);
}
