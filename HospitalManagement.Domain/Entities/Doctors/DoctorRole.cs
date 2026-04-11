using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities.Doctors;

public abstract class DoctorRole
{
    public Guid RoleId { get; private set; }
    public abstract string RoleName { get; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }

    // لتخزين جميع الرواتب مع الوقت 
    private readonly List<SalaryRecord> _salaryHistory = new();
    public IReadOnlyList<SalaryRecord> SalaryHistory => _salaryHistory.AsReadOnly();

    protected DoctorRole() { }

    // For creating new role 
    protected DoctorRole( Guid id , DateTime startDate)
    {
        RoleId = id;
        StartDate = startDate;
        IsActive = true;
    }
    // وقت تحميل البيانات
    protected DoctorRole(Guid id, DateTime startDate, DateTime? endDate , bool isActive)
    {
        RoleId = id;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }
    public void Deactivate(DateTime endDate)
    {
        if(IsActive == false)
            throw new InvalidOperationException($"Role '{RoleName}' is already inactive.");

        if(EndDate > endDate)
            throw new ArgumentException($"Role '{RoleName}' cannot be deactivated before its end date.");

        IsActive = false;
        EndDate = endDate;
    }

    public void ArchiveCurrentSalary(decimal amount)
    {
        _salaryHistory.Add(new SalaryRecord(DateTime.Now, amount));
    }
    public abstract decimal CalculateSalary(decimal referenceAmount);
}

public record SalaryRecord(DateTime Date, decimal Amount);
