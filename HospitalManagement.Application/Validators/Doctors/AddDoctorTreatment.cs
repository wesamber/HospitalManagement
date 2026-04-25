using FluentValidation;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Validators.Doctors;

public class AddDoctorTreatment : AbstractValidator<AddDoctorTreatmentDto>
{
    public AddDoctorTreatment()
    {
        
        RuleFor(x => x.TreatmentId)
            .NotEmpty().WithMessage("Treatment ID is required.");
                
        RuleFor(x => x.TreatmentRole)
            .NotEmpty().WithMessage("Treatment role is required.")
            .Must(role => Enum.TryParse<TreatmentRole>(role, ignoreCase: true, out _))
            .MaximumLength(50).WithMessage("Treatment role cannot exceed 50 characters.");
        
    }
}
