namespace HospitalManagement.Models;

public class DoctorTreatment
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    public int TreatmentId { get; set; }
    public TreatmentInternal Treatment { get; set; }
    public string RoleInTreatment { get; set; } // e.g., "Primary", "Assistant", etc.
}
