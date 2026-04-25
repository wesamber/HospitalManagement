using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors.DoctorRoles;

public class AddRoleDoctorDto
{
    public string RoleName { get; set; } = string.Empty; // Permanent , Contracted , Contarcted
    public DateTime StartDate { get; set; }

    // حسب النوع بيدخل الحقل
    public DateTime? EndDate { get; set; } // contractedRole
    public decimal? BaseSalary { get; set; } // PermanentRole
    public decimal? Percent { get; set; } // ContractedRole
}
