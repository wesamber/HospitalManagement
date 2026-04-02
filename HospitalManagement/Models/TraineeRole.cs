using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models;

public class TraineeRole : DoctorRole
{
    private readonly decimal _permanentBaseSalary;

    public TraineeRole() { }

    public TraineeRole(DateTime startDate, decimal permanentBaseSalary)
        : base(startDate)
    {
        _permanentBaseSalary = permanentBaseSalary;
    }

    public int GetYearLevel()
    {
        int years = (int)((DateTime.Now - StartDate).TotalDays / 365) + 1;
        return Math.Min(years, 2);
    }

    public bool IsEligibleForPromotion()
    {
        return (DateTime.Now - StartDate).TotalDays >= 365 * 2;
    }

    public override decimal CalculateSalary()
    {
        int year = GetYearLevel();

        if (year == 1)
            return _permanentBaseSalary * 0.50m;

        return _permanentBaseSalary * 0.75m;
    }
}
