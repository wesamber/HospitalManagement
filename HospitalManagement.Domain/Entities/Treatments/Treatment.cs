using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Treatments;

public abstract class Treatment : IEntity
{
    public Guid Id { get; private set; }
    public string NumberTreatment { get; protected set; } = string.Empty;
    public string Name { get; protected set; } = string.Empty;
    public DateTime StartDate { get;protected set; }
    public decimal Cost { get;protected set; }
    public Guid PatientId { get; private set; }

    public Treatment() { }

    // constructor for loading
    protected Treatment(Guid id, string numberTreatment, string name, DateTime startDate, decimal cost  , Guid patientId)
    {
        Id = id;
        NumberTreatment = numberTreatment;
        Name = name;
        StartDate = startDate;
        Cost = cost;
        PatientId = patientId;
    }

    // constructor for creating 
    protected Treatment(string numberTreatment,string name ,DateTime startDate, decimal cost , Guid patientId)
        : this(Guid.NewGuid(),name ,numberTreatment, startDate, cost, patientId)
    {   }

}

