using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Patients;

public class ExternalPatientDto : PatientDto
{
    public bool IsAdmitted { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public List<PatientTreatmentDto> ExternalTreatments { get; set; } = new();
}
