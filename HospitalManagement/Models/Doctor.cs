namespace HospitalManagement.Models;

public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public List<DoctorRole> Roles { get; set; } = new();
    public DoctorRole? ActiveRole => Roles.FirstOrDefault(r => r.IsActive);

    // Many-to-many relationship with Treatment
    public List<DoctorTreatment> Treatments { get; set; } = new ();
    public Doctor() { }

    public Doctor(int id, string name, string specialization)
    {
        DoctorId = id;
        Name = name;
        Specialization = specialization;
    }
    public void AddRole(DoctorRole role)
    {
        if (ActiveRole != null) ActiveRole.Deactivate(DateTime.Now);

        role.StartDate = DateTime.Now;
        role.EndDate = null;
        role.IsActive = true;
        Roles.Add(role);

    }
    //للترقية 
    public void PromoteToPermanent(decimal baseSalary)
    {
        if (ActiveRole != null)
            ActiveRole.Deactivate(DateTime.Now);

        var permanentRole = new PermanentRole(
            permanentSince: DateTime.Now, 
            baseSalary: baseSalary
        );

        permanentRole.StartDate = DateTime.Now;
        permanentRole.EndDate = null;
        permanentRole.IsActive = true;

        Roles.Add(permanentRole);
    }

    public decimal CalculateSalary()
    {
        return ActiveRole?.CalculateSalary() ?? 0;
    }

}
