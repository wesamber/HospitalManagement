using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Domain.Interfaces;

public interface ITreatmentRepository : IRepository<Treatment>
{
    IReadOnlyList<Treatment> GetByDate(DateTime dateStart , DateTime dateEnd);
}
