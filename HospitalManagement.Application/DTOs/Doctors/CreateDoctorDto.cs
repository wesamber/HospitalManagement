using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors;

public class CreateDoctorDto
{
    public string Name { get; set; } = string.Empty;

    // الenum بطبقة الdomain هون منحولا لنص مشان ما يكون في اعتماد بيناتن ع بعض 
    public string Specialization { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }

    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // properties for roles
    public string RoleName { get; set; } = string.Empty; // Permanent , Contracted , Contarcted
    public DateTime StartDate { get; set; }

    // حسب النوع بيدخل الحقل
    public DateTime? EndDate { get; set; } // contractedRole , كل الادوار اختياري 
    public decimal? BaseSalary { get; set; } // PermanentRole
    public decimal? Percent { get; set; } // ContractedRole
}
