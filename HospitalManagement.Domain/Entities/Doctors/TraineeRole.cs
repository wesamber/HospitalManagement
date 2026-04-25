using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Doctors;

public class TraineeRole : DoctorRole 
{
    public override string RoleName => "trainee";

    private TraineeRole() { }

    // constructor for creating
    public TraineeRole(DateTime startDate , DateTime? endDate , bool isActive = true)
        : base(Guid.NewGuid(), startDate, endDate, isActive) { }


    // constructor for loading
    public TraineeRole(Guid id, DateTime startDate, DateTime? endDate, bool isActive)
        : base(id, startDate , endDate , isActive)   { }

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
