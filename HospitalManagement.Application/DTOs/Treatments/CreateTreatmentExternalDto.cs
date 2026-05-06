using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class CreateTreatmentExternalDto
{
    public Guid PatientId { get; set; }
    public decimal Cost { get; set; }
    public DateTime StartDate { get; set; }
    public string ClinicNumber { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
}
