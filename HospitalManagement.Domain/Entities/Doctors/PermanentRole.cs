using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Doctors;

public class PermanentRole : DoctorRole
{
    public override string RoleName => "permanent";
    public decimal BaseSalary { get;private set; }

    private PermanentRole() { }

    // constructor for creating
    public PermanentRole(DateTime startDate ,DateTime? endDate ,decimal baseSalary)
        : base(Guid.NewGuid(), startDate , endDate , true)
    {
        BaseSalary = baseSalary;
    }

    // constructor for loading
    public PermanentRole(Guid id, DateTime startDate,DateTime? endDate,bool isActive , decimal baseSalary)
        : base(id, startDate , endDate , isActive)
    {
        BaseSalary = baseSalary;
    }
    private int YearsAsPermanent()
    {
        return (int)((DateTime.Now - StartDate).TotalDays / 365);
    }
    public override decimal CalculateSalary(decimal systemBaseSalary)
    {
        // اذا كان الراتب الأساسي في النظام أقل من راتب الطبيب الدائم، نستخدم راتب الطبيب كقاعدة للحساب
        decimal effectiveBase = Math.Max(this.BaseSalary, systemBaseSalary);
        int increments = YearsAsPermanent() / 2;
        return effectiveBase * (1 + 0.10m * increments);
    }
}
