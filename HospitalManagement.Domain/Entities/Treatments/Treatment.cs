using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Treatments;

public abstract class Treatment : IEntity
{
    public Guid Id { get; private set; }
    public string NumberTreatment { get; protected set; } = string.Empty;
    public DateTime StartDate { get;protected set; }
    public decimal Cost { get;protected set; }

    protected Treatment() { }

    // constructor for loading
    protected Treatment(Guid id, string numberTreatment, DateTime startDate, decimal cost)
    {
        Id = id;
        NumberTreatment = numberTreatment;
        StartDate = startDate;
        Cost = cost;
    }

    // constructor for creating 
    protected Treatment(string numberTreatment, DateTime startDate, decimal cost)
        : this(Guid.NewGuid(), numberTreatment, startDate, cost)
    {   }

}

