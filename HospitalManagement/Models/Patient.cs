namespace HospitalManagement.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public List<Treatment> Treatments { get; set; } = new();

    protected Patient() { }
    protected Patient(int id,string name, DateOnly dateOfBirth, string address, string numbre, string email)
    {
        PatientId = id;
        Name = name;
        DateOfBirth = dateOfBirth;
        Address = address;
        PhoneNumber = numbre;
        Email = email;
    }

}
