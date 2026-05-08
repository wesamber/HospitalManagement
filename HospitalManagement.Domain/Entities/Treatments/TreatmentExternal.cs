namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentExternal : Treatment
{
    public string ClinicNumber { get;private set; } = string.Empty;
    public string DoctorName { get;private set; } = string.Empty;

    public TreatmentExternal() { }

    // constructor for loading
    public TreatmentExternal(
        Guid id, string numberTreatment, DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(id, numberTreatment, startDate, cost , patientId)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
    }

    //constructor for creating
    public TreatmentExternal(
        string numberTreatment, DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(numberTreatment, startDate, cost , patientId)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
    }
}
