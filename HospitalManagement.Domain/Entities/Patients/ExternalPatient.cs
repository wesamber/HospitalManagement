using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Patients;

public class ExternalPatient : Patient  , IEntity
{
    public bool IsAdmitted { get;private set; } = false;
    public DateTime? AdmissionDate { get; private set; }

    protected ExternalPatient() { }

    public ExternalPatient(
        string patientNumber,string name,
        DateOnly? dateOfBirth,string? address,
        string? phone,string? email)
        : base(patientNumber, name, dateOfBirth, address, phone, email)
    {
    }

    public ExternalPatient(Guid patientId , string patientNumber, string name, 
        DateOnly? dateOfBirth, string? address, 
        string? phone, string? email,
        IEnumerable<Guid>? externalTreatmentIds , bool isAdmitted,
        DateTime? admissionDate)
        : base(patientId, patientNumber, name, dateOfBirth, address, phone, email , externalTreatmentIds)
    {
        IsAdmitted = isAdmitted;
        AdmissionDate = admissionDate;
    }


    public void Admit()
    {
        if(IsAdmitted) 
            throw new InvalidOperationException("Patient is already admitted.");

        IsAdmitted = true;
        AdmissionDate = DateTime.Now;
        // تغيير النوع بيصير بطبقة ال application 
    }
}
