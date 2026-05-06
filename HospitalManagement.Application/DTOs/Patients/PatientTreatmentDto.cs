using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Patients;
public class PatientTreatmentDto
{
    public Guid TreatmentId { get; set; }
    public string TreatmentNumber { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
