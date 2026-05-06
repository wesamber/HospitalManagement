using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Configuration;

public class JsonPathsOptions
{
    public string DoctorsSnapshot { get; set; } = string.Empty;
    public string DoctorsLog { get; set; } = string.Empty;
    public string DepartmentsSnapshot { get; set; } = string.Empty;
    public string DepartmentsLog { get; set; } = string.Empty;
    public string PatientsSnapshot { get; set; } = string.Empty;
    public string PatientsLog { get; set; } = string.Empty;
    public string TreatmentsSnapshot { get; set; } = string.Empty;
    public string TreatmentsLog { get; set; } = string.Empty;

}
