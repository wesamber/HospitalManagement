using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models;

public abstract class DoctorRole
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive => EndDate == null || EndDate > DateTime.Now;

    public abstract decimal CalculateSalary();
}
