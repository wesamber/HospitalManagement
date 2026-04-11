using HospitalManagement.Domain.Entities.Enums;
using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Domain.Entities.Doctors;

public class DoctorTreatment
{
    public Guid DoctorId { get; set; }
    public Guid TreatmentId { get; set; }
    public TreatmentRole RoleInTreatment { get; set; }

    protected DoctorTreatment() { }
    public DoctorTreatment(Guid doctorId, Guid treatmentId, TreatmentRole role)
    {
        DoctorId = doctorId;
        TreatmentId = treatmentId;
        RoleInTreatment = role;
    }
}
