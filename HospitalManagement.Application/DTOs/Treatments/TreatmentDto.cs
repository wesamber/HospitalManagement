using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class TreatmentDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; } 
    public decimal Cost { get; set; } 

}
