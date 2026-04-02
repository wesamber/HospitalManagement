namespace HospitalManagement.Models;

public class TreatmentInternal : Treatment
{
    public string Diagnosis { get; set; }
    public int DepartmentId { get; set; }
    public decimal Cost { get; set; }

    // جدول الكسر
    public List<DoctorTreatment> DoctorLinks { get; set; } = new();
    public TreatmentInternal() { }

    public TreatmentInternal(int treatmentId, DateTime date, string notes,
                                  string diagnosis, decimal cost,int deptid)
            : base(treatmentId, date, notes, cost)
    {
        Diagnosis = diagnosis;
        Cost = cost;
        DepartmentId = deptid;
    }
    



    
}
