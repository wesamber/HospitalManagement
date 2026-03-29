namespace HospitalManagement.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Foreign key to Treatment
    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = new();
}
