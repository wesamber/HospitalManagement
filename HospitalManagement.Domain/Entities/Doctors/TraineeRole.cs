using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities.Doctors;

public class TraineeRole : DoctorRole
{
    public override string RoleName => "Trainee";

    public TraineeRole(Guid id, DateTime startDate)
        : base(id, startDate)   { }

    public int GetYearLevel()
    {
        int years = (int)((DateTime.Now - StartDate).TotalDays / 365) + 1;
        return Math.Min(years, 2);
    }

    // هل صار وقت الترقية؟
    public bool IsEligibleForPromotion() 
    {
        return (DateTime.Now - StartDate).TotalDays >= 365 * 2;
    }

    public override decimal CalculateSalary(decimal systemBaseSalary)
    {
        int year = GetYearLevel();

        if (year == 1)
            return systemBaseSalary * 0.50m;

        return systemBaseSalary * 0.75m;
    }
}
