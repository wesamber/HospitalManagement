namespace HospitalManagement.Models;

public abstract class Treatment
{
    public int TreatmentId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public decimal Cost { get; set; }

    protected Treatment() { }

    protected Treatment(int id, DateTime date, string notes, decimal cost)
    {
        TreatmentId = id;
        Date = date;
        Notes = notes;
        Cost = cost;
    }

   
}

