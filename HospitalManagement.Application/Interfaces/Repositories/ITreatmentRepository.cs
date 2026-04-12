using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface ITreatmentRepository : IRepository<Treatment>
{
    IReadOnlyList<Treatment> GetByDate(DateTime dateStart , DateTime dateEnd);
}
