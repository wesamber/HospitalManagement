using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;
public static class DoctorJsonModelMapper
{
    public static DoctorJsonModel ToJsonModel(this Doctor doctor)
    {
        return new DoctorJsonModel
        {
            Id = doctor.Id,
            Name = doctor.Name,
            DoctorNumber = doctor.DoctorNumber,
            Specialization = doctor.Specialization.ToString(),
            DateOfBirth = doctor.DateOfBirth,
            Address = doctor.Address,
            PhoneNumber = doctor.PhoneNumber,
            Email = doctor.Email,

            DepartmentsIds = doctor.DepartmentsIds.ToList(),

            Treatments = doctor.Treatments.Select(t => new DoctorTreatmentJsonModel
            {
                DoctorId = t.DoctorId,
                TreatmentId = t.TreatmentId,
                RoleInTreatment = t.RoleInTreatment.ToString()
            }).ToList(),

            Roles = doctor.Roles.Select(ToJsonModel).ToList()
        };
    }

    public static Doctor ToDomain(this DoctorJsonModel model)
    {
        if (!Enum.TryParse<Specialization>(model.Specialization, ignoreCase: true, out var specialization))
            throw new ArgumentException($"Invalid specialization value '{model.Specialization}'.", nameof(model));

        var doctor = new Doctor(
            id: model.Id,
            name: model.Name,
            doctorNumber: model.DoctorNumber,
            specialization: specialization,
            dob: model.DateOfBirth,
            address: model.Address,
            phoneNumber: model.PhoneNumber,
            email: model.Email
            );

        // استخدمت ال internal للتحميل بدون التحقق لانو البيانات جاية من التخزين
        doctor.LoadDepartmentIds(model.DepartmentsIds);

        var treatments = model.Treatments.Select(t => new DoctorTreatment(
        doctorId: t.DoctorId,
        treatmentId: t.TreatmentId,
        role: Enum.Parse<TreatmentRole>(t.RoleInTreatment, ignoreCase: true))).ToList();

        doctor.LoadTreatments(treatments);

        var roles = model.Roles
                     .Select(ToDomain)
                     .OrderBy(r => r.StartDate).ToList();
        
        doctor.LoadRoles(roles);

        return doctor;
    }

    private static DoctorRoleJsonModel ToJsonModel(DoctorRole role)
    {
        var jsonModel = new DoctorRoleJsonModel
        {
            Id = role.Id,
            RoleType = role.RoleName,
            StartDate = role.StartDate,
            EndDate = role.EndDate,
            IsActive = role.IsActive,
            SalaryHistory = role.SalaryHistory
                .Select(s => new SalaryRecordJsonModel
                {
                    RecordedAt = s.Date,
                    Amount = s.Amount
                })
                .ToList()
        };

        if (role is PermanentRole permanentRole)
            jsonModel.BaseSalary = permanentRole.BaseSalary;

        if (role is ContractedRole contractedRole)
        {
            jsonModel.Percent = contractedRole.Percent;
            jsonModel.TreatmentIds = contractedRole.TreatmentIds.ToList();
        }

        return jsonModel;
    }

    private static DoctorRole ToDomain(DoctorRoleJsonModel model)
    {
        DoctorRole role = model.RoleType.Trim().ToLowerInvariant() switch
        {
            "permanent" => new PermanentRole(
                id: model.Id,
                startDate: model.StartDate,
                endDate: model.EndDate,
                isActive: model.IsActive,
                baseSalary: model.BaseSalary ?? 0m),

            "contracted" => new ContractedRole(
                id: model.Id,
                startDate: model.StartDate,
                endDate: model.EndDate,
                isActive: model.IsActive,
                percent: model.Percent ?? 0.5m),

            "trainee" => new TraineeRole(
                id: model.Id,
                startDate: model.StartDate,
                endDate: model.EndDate,
                isActive: model.IsActive),

            _ => throw new ArgumentException($"Unknown doctor role type '{model.RoleType}'.", nameof(model))
        };

            foreach(var salary in model.SalaryHistory)
                role.LoadSalaryRecord(new SalaryRecord(salary.RecordedAt, salary.Amount));

        return role;
    }
}
