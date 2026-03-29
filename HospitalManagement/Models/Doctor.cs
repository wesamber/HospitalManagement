namespace HospitalManagement.Models;

public class Doctor
{
    public int Id { get; set; }
    public string DoctorNumber { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public List<DoctorRole> Roles { get; set; } = new List<DoctorRole>();
    public DoctorRole? ActiveRole => Roles.FirstOrDefault(r => r.IsActive);

    //// Many-to-many relationship with Treatment
    //public List<DoctorTreatments> Treatments { get; set; } = new List<DoctorTreatments>();

    public void AddRole(DoctorRole role)
    {
        var current = ActiveRole; // search for the currently active role once only
        if (current != null)
        {
            current.EndDate = DateTime.Now;
        }
        Roles.Add(role);
    }

}
