using HospitalManagement.Domain.Entities.Enums;
namespace HospitalManagement.Domain.Entities.Doctors;

public class DoctorTreatment
{
    public Guid DoctorId { get;private set; }
    public Guid TreatmentId { get;private set; }
    public TreatmentRole RoleInTreatment { get;private set; }

    public DoctorTreatment() { }
    public DoctorTreatment(Guid doctorId, Guid treatmentId, TreatmentRole role)
    {
        DoctorId = doctorId;
        TreatmentId = treatmentId;
        RoleInTreatment = role;
    }
}
