namespace HospitalManagement.Models;

public class Treatment
{
    public int Id { get; set; }
    public DateOnly TreatmentDate { get; set; }
    public string? Description { get; set; }
    public decimal Cost { get; set; }

    // Many-to-many relationship with Doctor
    public List<DoctorTreatments> Doctor { get; set; } = new List<DoctorTreatments>();
}
