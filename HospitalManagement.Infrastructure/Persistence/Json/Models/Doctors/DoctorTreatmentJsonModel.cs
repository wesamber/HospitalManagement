using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;

public class DoctorTreatmentJsonModel
{
    public Guid DoctorId { get; set; }
    public Guid TreatmentId { get; set; }
    public string RoleInTreatment { get; set; } = string.Empty;
}
