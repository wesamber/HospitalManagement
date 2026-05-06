using FluentValidation;
using HospitalManagement.Application.DTOs.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Validators.Patients;

public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date of birth must be in the past.")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
