using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Parient;

public class PatientJsonModel
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // internal - external
    public string PatientNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public List<Guid> ExternalTreatmentIds { get; set; } = new();

    // Internal only
    public bool IsDischarged { get; set; }
    public DateTime? DischargeDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<Guid> InternalTreatmentIds { get; set; } = new();

    // External only
    public bool IsAdmitted { get; set; }
    public DateTime? AdmissionDate { get; set; }
}
