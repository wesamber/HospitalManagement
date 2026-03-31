using HospitalManagement.Models.Contracts;

namespace HospitalManagement.Models;

public class PermanentRole : DoctorRole
{
    public decimal BaseSalary { get; set; }
    public DateTime PermanentSince { get; set; }

    public PermanentRole() { }

    public PermanentRole(DateTime permanentSince, decimal baseSalary)
        : base(permanentSince)   // StartDate = permanentSince للدور الأول
    {
        BaseSalary = baseSalary;
        PermanentSince = permanentSince;
    }
    private int YearsAsPermanent()
    {
        return (int)((DateTime.Now - PermanentSince).TotalDays / 365);
    }
    public override decimal CalculateSalary()
    {
        int increments = YearsAsPermanent() / 2;
        return BaseSalary * (1 + 0.10m * increments);
    }
}
