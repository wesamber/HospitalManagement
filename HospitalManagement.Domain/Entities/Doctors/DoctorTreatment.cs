using HospitalManagement.Domain.Entities.Enums;
namespace HospitalManagement.Domain.Entities.Doctors;

public class DoctorTreatment
{
    public Guid DoctorId { get;private set; }
    public Guid TreatmentId { get;private set; }
    public enTreatmentRole RoleInTreatment { get;private set; }

    protected DoctorTreatment() { }
    public DoctorTreatment(Guid doctorId, Guid treatmentId, enTreatmentRole role)
    {
        DoctorId = doctorId;
        TreatmentId = treatmentId;
        RoleInTreatment = role;
    }
}
