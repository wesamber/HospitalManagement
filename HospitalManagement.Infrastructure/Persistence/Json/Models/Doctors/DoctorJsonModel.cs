namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;

public class DoctorJsonModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DoctorNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public List<Guid> DepartmentsIds { get; set; } = new();
    public List<DoctorTreatmentJsonModel> Treatments { get; set; } = new();
    public List<DoctorRoleJsonModel> Roles { get; set; } = new();

    public DoctorJsonModel() { }
}
