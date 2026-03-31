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
    public bool IsActive { get; set; }

    protected DoctorRole() { }

    protected DoctorRole(DateTime startDate)
    {
        StartDate = startDate;
        IsActive = true;
        EndDate = null;
    }
    public void Deactivate(DateTime endDate)
    {
        IsActive = false;
        EndDate = endDate;
    }
    public abstract decimal CalculateSalary();
}
