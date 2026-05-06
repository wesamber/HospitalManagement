using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class TreatmentListDto
{
    public Guid Id { get; set; }
    public string NumberTreatment { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "internal" / "external"
    public DateTime StartDate { get; set; }
    public decimal Cost { get; set; }
    public Guid PatientId { get; set; }
}
