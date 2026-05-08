using HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Treatments;

public class TreatmentJsonModel
{
    public Guid Id { get; set; }
    public string NumberTreatment { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal Cost { get; set; }
    public Guid PatientId { get; set; }
    public string Type { get; set; } = string.Empty;

    // Internal only
    public DateTime? DateDischarge { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<DoctorTreatmentJsonModel> Doctors { get; set; } = new();

    // External only
    public string? ClinicNumber { get; set; }
    public string? DoctorName { get; set; }
}
