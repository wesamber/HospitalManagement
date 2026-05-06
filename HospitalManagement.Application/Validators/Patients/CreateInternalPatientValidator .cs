using FluentValidation;
using HospitalManagement.Application.DTOs.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Validators.Patients;
public class CreateInternalPatientValidator : AbstractValidator<CreatePatientInternalDto>
{
    public CreateInternalPatientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Patient name is required.")
            .MaximumLength(100).WithMessage("Patient name must not exceed 100 characters.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department ID is required.");

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
