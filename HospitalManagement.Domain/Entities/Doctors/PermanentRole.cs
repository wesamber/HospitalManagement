
namespace HospitalManagement.Domain.Entities.Doctors;

public class PermanentRole : DoctorRole
{
    public override string RoleName => "Permanent";
    public decimal BaseSalary { get;private set; }

    protected PermanentRole() { }

    public PermanentRole(Guid id, DateTime startDate, decimal baseSalary)
        : base(id, startDate)
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
