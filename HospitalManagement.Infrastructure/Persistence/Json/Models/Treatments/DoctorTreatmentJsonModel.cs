using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Treatments;

public class DoctorTreatmentJsonModel
{
    public Guid DoctorId { get; set; }
    public Guid TreatmentId { get; set; }
    public string Role { get; set; } = string.Empty;
}
