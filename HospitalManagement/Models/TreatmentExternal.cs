namespace HospitalManagement.Models;

public class TreatmentExternal : Treatment
{
    public int ClinicNumber { get; set; }
    public Doctor Doctor { get; set; }

    public TreatmentExternal() { }
    public TreatmentExternal(int id, DateTime date, string notes, decimal cost,
                             int clinicNumber, Doctor doctor)
        : base(id, date, notes, cost)
    {
        ClinicNumber = clinicNumber;
        Doctor = doctor;
    }
}
