using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models
{
    public class Department
    {
        public int DeptId { get; set; }
        public string Name { get; set; }

        public List<InternalPatient> Patients { get; set; } = new List<InternalPatient>();

        public Department() { }

        public Department(int deptId, string name)
        {
            DeptId = deptId;
            Name = name;
        }

        public void AdmitPatient(InternalPatient patient)
        {
            Patients.Add(patient);
            Console.WriteLine($"  Patient {patient.Name} admitted to department '{Name}'.");
        }

        public void DischargePatient(int patientId)
        {
            var patient = Patients.Find(p => p.PatientId == patientId);
            if (patient != null)
            {
                patient.Discharge();
                Patients.Remove(patient);
            }
            else
            {
                Console.WriteLine($"  Patient #{patientId} not found in department '{Name}'.");
            }
        }

       
        public int CountPatientsInPeriod(DateTime from, DateTime to)
        {
            int count = 0;
            foreach (var p in Patients)
            {
                foreach (var t in p.InternalTreatments)
                {
                    if (t.Date >= from && t.Date <= to)
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }
    }
}
