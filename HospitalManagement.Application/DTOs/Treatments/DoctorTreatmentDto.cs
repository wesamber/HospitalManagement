using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class DoctorTreatmentDto
{
    public Guid DoctorId { get; set; }
    public Guid TreatmentId { get; set; }
    public string TreatmentRole { get; set; } = string.Empty;
    // ما حطيت ال trreatmentid لان حيكون موجود في ال treatment internal dto
}
