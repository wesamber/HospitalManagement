using FluentValidation;
using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Validators.Treatments;

public class AssignDoctorValidator : AbstractValidator<AssignDoctorDto>
{
    public AssignDoctorValidator()
    {
        RuleFor(x => x.TreatmentId)
            .NotEmpty().WithMessage("TreatmentId is required.");
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required.");
        RuleFor(x => x.RoleInTreatment)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => Enum.TryParse(role, ignoreCase: true, out TreatmentRole _))
            .WithMessage("Role must be a valid value of RoleInTreatment enum.");
    }
}
