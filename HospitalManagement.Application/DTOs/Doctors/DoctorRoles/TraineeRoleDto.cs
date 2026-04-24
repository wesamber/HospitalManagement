using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors.DoctorRoles;

public class TraineeRoleDto : DoctorRoleDto
{
    public int YearLevel { get; set; }
    public bool IsEligibleForPromotion { get; set; }
}
