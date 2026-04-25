using FluentValidation;
using FluentValidation.Validators;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Validators.Doctors;

public class AddRoleDoctor : AbstractValidator<AddRoleDoctorDto>
{
    private readonly string[] _allowedRoles =  { "contracted", "trainee", "permanent" };
    public AddRoleDoctor()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .Must(x => _allowedRoles.Contains(x?.ToLower()))
            .WithMessage("Invalid role name. Allowed roles are: contracted, trainee, permanent.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");
    }
}
