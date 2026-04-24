using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Patients;

public class CreatePatientInternalDto : CreatePatientDto
{
    public Guid DepartmentId { get; set; }
}
