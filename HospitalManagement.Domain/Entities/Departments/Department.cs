using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities.Departments
{
    public class Department
    {
        public Guid DepartmentId { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private readonly List<InternalPatient> _patients = new();
        public IReadOnlyList<InternalPatient> Patients => _patients.AsReadOnly();

        private readonly List<Doctor> _doctors = new();
        public IReadOnlyList<Doctor> Doctors => _doctors.AsReadOnly();

        private Department() { }
        // For creating new department with just a name
        public Department(string name)
        {
            DepartmentId = Guid.NewGuid();
            Name = name;
        }

        public Department(
            string name , 
            IEnumerable<InternalPatient> patients, 
            IEnumerable<Doctor> doctors)
        {
            DepartmentId = Guid.NewGuid();
            Name = name;
            _patients.AddRange(patients);
            _doctors.AddRange(doctors);
        }

        public void AdmitPatient(InternalPatient patient)
        {
            if(!_patients.Contains(patient))
                _patients.Add(patient);
        }

        public void DischargePatient(Guid patientId , DateTime date)
        {
            var patient = _patients.FirstOrDefault(p => p.PatientId == patientId);
            if(patient is null)
                throw new InvalidOperationException($"Patient with ID {patientId} not found in department '{Name}'.");
            patient.Discharge(date);
        }

        //public int CountPatientsInPeriod(DateTime from, DateTime to)
        //     => _patients.Count(p =>
        //    p.InternalTreatments.Any(t => t.DateStart >= from && t.DateStart     <= to));
        
    }
}
