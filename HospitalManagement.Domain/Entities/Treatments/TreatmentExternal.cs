namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentExternal : Treatment
{
    public string ClinicNumber { get;private set; } = string.Empty;
    public string DoctorName { get;private set; } = string.Empty;
    public Guid PatientId { get;private set; }

    public TreatmentExternal() { }

    // constructor for loading
    public TreatmentExternal(
        Guid id, string numberTreatment, DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(id, numberTreatment, startDate, cost)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
        PatientId = patientId;
    }

    //constructor for creating
    public TreatmentExternal(
        string numberTreatment, DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(numberTreatment, startDate, cost)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
        PatientId = patientId;
    }
}
