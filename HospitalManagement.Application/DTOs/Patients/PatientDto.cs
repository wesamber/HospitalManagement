using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Patients;

public abstract class PatientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;  
    public string PatientNumber { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<Guid> ExternalTreatmentIds { get; set; } = new();
}
