namespace HospitalManagement.Models;

public class DoctorTreatments
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = new();
    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = new();
    public string? Role { get; set; } // e.g., "Primary", "Assistant", etc.
}
