namespace HospitalManagement.Domain.Entities.Treatments;

public abstract class Treatment
{
    public Guid TreatmentId { get; private set; }
    public DateTime Date { get;protected set; }
    public decimal Cost { get;protected set; }

    protected Treatment() { }

    // constructor for loading
    protected Treatment(Guid id , DateTime date,  decimal cost)
    {
        TreatmentId = id;
        Date = date;
        Cost = cost;
    }

    // constructor for creating 
    protected Treatment(DateTime date, decimal cost) : this(Guid.NewGuid(), date, cost)
    {   }

}

