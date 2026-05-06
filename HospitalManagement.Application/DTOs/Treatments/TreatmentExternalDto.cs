using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class TreatmentExternalDto : TreatmentListDto
{
    public string ClinicNumber { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
}
