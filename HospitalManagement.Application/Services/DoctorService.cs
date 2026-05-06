using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Doctors;
using HospitalManagement.Application.Validators.Doctors;
using HospitalManagement.Domain.Entities.Config;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<CreateDoctorDto> _createValidator;
    private readonly IValidator<UpdateDoctorDto> _updateValidator;
    private readonly IValidator<AddRoleDoctorDto> _addRoleValidator;
    private readonly ISystemConfigRepository _configRepository;
    private readonly INumberGenerator _numberGenerator;
    private readonly IMapper _mapper;
    public DoctorService(
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        ITreatmentRepository treatmentRepository,
        IValidator<CreateDoctorDto> createValidator,
        IValidator<UpdateDoctorDto> updateValidator,
        IValidator<AddRoleDoctorDto> addRoleValidator,
        ISystemConfigRepository systemConfigRepository,
        INumberGenerator numberGenerator,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _treatmentRepository = treatmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addRoleValidator = addRoleValidator;
        _configRepository = systemConfigRepository;
        _numberGenerator = numberGenerator;
        _mapper = mapper;
    }
    #region CRUD Operations Doctors
    public async Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto)
    {
        //1. validation for doctor data
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return Result<DoctorDto>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                );
        }

        // split add role doctor from create doctor
        var addRoleDoctorDto = dto.SplitRoleFromDto();
        
        //1.2 validate for Role doctor
        var validationRoleResult = await _addRoleValidator.ValidateAsync(addRoleDoctorDto);

        if (!validationRoleResult.IsValid)
        {
            return Result<DoctorDto>.Failure(
                    validationRoleResult.Errors.Select(e => e.ErrorMessage).ToList()
                );
        }

        // 2. check if doctor is exist
        var existingDoctor = await _doctorRepository.ExistAsync(dto.Name , dto.DateOfBirth);

        if (existingDoctor)
        {
            return Result<DoctorDto>.Failure("Doctor already exists.");
        }

        // الراتب الاساسي بالمشفى 
        var config = await _configRepository.GetAsync();

        //3. create role doctor
        var initialRole = CreateInitialRole(addRoleDoctorDto, config);

        //3.1 archive role that created
        ArchiveInitialSalary(initialRole, config.BaseSalary);

        //4. Generate doctor number
        var doctorNumber = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Doctor);

        //5. Create doctor (dto to entity)
        var doctor = dto.ToEntity(doctorNumber);

        //6. add role doctor to doctor
        doctor.AddRole(initialRole);

        //7. Save to database
        await _doctorRepository.AddAsync(doctor);

        //8. convert to dto ( entity to dto)
        var doctorDto = _mapper.Map<DoctorDto>(doctor);

        return Result<DoctorDto>.SuccessResult(doctorDto);
        
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        // 1. Check if doctor exists
        var doctor = await _doctorRepository.GetByIdAsync(id);

        if (doctor == null)
        {
            return Result<bool>.Failure("Doctor not found.");
        }

        // 2. Delete doctor
        await _doctorRepository.DeleteAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<List<DoctorDto>>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);
        return Result<List<DoctorDto>>.SuccessResult(doctorDtos);
    }

    public async Task<Result<DoctorDetailsDto>> GetByIdAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null)
            return Result<DoctorDetailsDto>.Failure("Doctor not found.");

        var dto = _mapper.Map<DoctorDetailsDto>(doctor);

        // جيب كائنات الأقسام
        var departments = new List<DepartmentDoctorDto>();
        foreach (var deptId in doctor.DepartmentsIds)
        {
            var dept = await _departmentRepository.GetByIdAsync(deptId);
            if (dept != null)
                departments.Add(new DepartmentDoctorDto
                {
                    Id = dept.Id,
                    Name = dept.Name,
                });
        }

        dto.Departments = departments;

        return Result<DoctorDetailsDto>.SuccessResult(dto);
    }

    public async Task<Result<bool>> UpdateAsync(UpdateDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(dto.Id);

        if(doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return Result<bool>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                );
        }
        if (!Enum.TryParse<Specialization>(dto.Specialization, true, out var specialization))
        {
            return Result<bool>.Failure("Invalid specialization value.");
        }
        // Update doctor properties
        doctor.UpdateInfo(dto.Name,specialization, dto.DateOfBirth, dto.Address, dto.PhoneNumber, dto.Email);

        if (dto.Percent.HasValue)
        {
            if (doctor.ActiveRole is not ContractedRole contractedRole)
                return Result<bool>.Failure("Percent can only be updated for contracted doctors.");

            contractedRole.UpdatePercent(dto.Percent.Value);
        }

        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }
    #endregion

    #region GET By Properties
    public async Task<Result<List<DoctorDto>>> GetBySpecializationAsync(string specialization)
    {
        if (!Enum.TryParse<Specialization>(specialization, true, out var specializationEnum))
        {
            return Result<List<DoctorDto>>.Failure("Invalid specialization value.");
        }
        var doctors = await _doctorRepository.GetBySpecializationAsync(specializationEnum);
        var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);
        return Result<List<DoctorDto>>.SuccessResult(doctorDtos);
    }

    public async Task<Result<List<DoctorDto>>> GetByDepartmentAsync(Guid departmentId)
    {
        var doctors = await _doctorRepository.GetByDepartmentAsync(departmentId);
        var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);
        return Result<List<DoctorDto>>.SuccessResult(doctorDtos);
    }

    public async Task<Result<DoctorDetailsDto>> GetByNumberAsync(string doctorNumber)
    {
        var doctor = await _doctorRepository.GetByNumberAsync(doctorNumber);
        if (doctor == null)
        {
            return Result<DoctorDetailsDto>.Failure("Doctor not found.");
        }
        var doctorDetailsDto = _mapper.Map<DoctorDetailsDto>(doctor);
        return Result<DoctorDetailsDto>.SuccessResult(doctorDetailsDto);
    }
    #endregion

    #region METHODS DOMAIN
    public async Task<Result<bool>> AssignToDepartmentAsync(Guid doctorId , Guid departmentId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
        {
            return Result<bool>.Failure("Doctor not found.");
        }

        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if(department == null)
        {
            return Result<bool>.Failure("Department not found.");
        }

        doctor.AssignToDepartment(departmentId);
        department.AssignDoctor(doctorId);
        await _doctorRepository.UpdateAsync(doctor);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> AddTreatmentToDoctorAsync(Guid doctorId , AddDoctorTreatmentDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        if (!Enum.TryParse<TreatmentRole>(dto.TreatmentRole, true, out var role))
            return Result<bool>.Failure("Invalid treatment role.");

        var treatment = await _treatmentRepository.GetByIdAsync(dto.TreatmentId);
        if (treatment == null)
            return Result<bool>.Failure("Treatment not found.");

        if(doctor.Treatments.Any(dt => dt.TreatmentId == dto.TreatmentId))
            return Result<bool>.Failure("Doctor is already assigned to this treatment.");

        if (treatment is not TreatmentInternal internalTreatment)
            return Result<bool>.Failure("Only internal treatments can have assigned doctors.");

        var treatmentDoctor = new DoctorTreatment(doctorId, dto.TreatmentId, role);

        // ربط من طرف الدكتور
        doctor.AddTreatment(treatmentDoctor);
        await _doctorRepository.UpdateAsync(doctor);

        // ربط من طرف المعالجة
        internalTreatment.AddDoctor(treatmentDoctor);
        await _treatmentRepository.UpdateAsync(internalTreatment);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<decimal>> CalculateSalaryAsync(Guid doctorId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<decimal>.Failure("Doctor not found.");

        decimal currentSalary = 0;

        if (doctor.ActiveRole is ContractedRole)
        {
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var doctorTreatmentIds = doctor.Treatments
                .Select(dt => dt.TreatmentId)
                .ToList();

            var allTreatments = await _treatmentRepository.GetByIdsAsync(doctorTreatmentIds);

            var monthlyTreatments = allTreatments
                .Where(t => t.StartDate >= monthStart && t.StartDate <= now)
                .ToList();

            var referenceAmount = monthlyTreatments.Sum(t => t.Cost);
            currentSalary = doctor.CalculateSalary(referenceAmount);
        }
        else
        {
            var config = await _configRepository.GetAsync();
            currentSalary = doctor.CalculateSalary(config.BaseSalary);
        }

        return Result<decimal>.SuccessResult(currentSalary);
    }

    public async Task<Result<bool>> PromoteDoctorToPermanentAsync(Guid doctorId, decimal? baseSalary = null)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        var config = await _configRepository.GetAsync();
        var finalSalary = baseSalary ?? config.BaseSalary;

        doctor.PromoteToPermanent(finalSalary);
        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> AddRoleToDoctorAsync(Guid doctorId, AddRoleDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        // إذا كان الدور الحالي متعاقد، خزّن راتبه المتراكم قبل إضافة دور جديد
        if (doctor.ActiveRole is ContractedRole contractedRole)
        {
            await ArchiveCurrentMonthSalaryForContractedAsync(doctor.Id, contractedRole);
        }

        var config = await _configRepository.GetAsync();

        // create new role and archive his salary
        var newRole = CreateInitialRole(dto, config);
        ArchiveInitialSalary(newRole, config.BaseSalary);

        doctor.AddRole(newRole);
        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }
    #endregion

    #region Private Methods
    private DoctorRole CreateInitialRole(AddRoleDoctorDto dto , SystemConfig systemConfig)
    {
        // ContractedRole.Percent must be a ratio between 0 and 1 (e.g., 0.5 = 50%).
        if (dto.RoleName.Equals("contracted", StringComparison.OrdinalIgnoreCase) && dto.Percent is not null)
        {
            if (dto.Percent <= 0m || dto.Percent > 1m)
                throw new Exception("Percent for contracted role must be between 0 and 1.");
        }

        DoctorRole role = dto.RoleName.ToLower() switch
        {
            "permanent" => new PermanentRole(dto.StartDate, dto.EndDate, dto.BaseSalary ?? systemConfig.BaseSalary),
            "contracted" => new ContractedRole(dto.StartDate, dto.EndDate, dto.Percent ?? 0.5m),
            "trainee" => new TraineeRole(dto.StartDate, dto.EndDate),
            _ => throw new Exception("Invalid Role Type")
        };

        return role;
    }

    private void ArchiveInitialSalary(DoctorRole role, decimal systemBaseSalary)
    {
        // SalaryHistory should only store monetary monthly amounts.
        // For ContractedRole, salary depends on treatments and is archived at month-end or before role change.
        if (role is ContractedRole)
            return;

        decimal amountToArchive = role switch
        {
            PermanentRole p => p.BaseSalary,
            TraineeRole t => t.CalculateSalary(systemBaseSalary),
            _ => 0m
        };

        role.ArchiveCurrentSalary(amountToArchive);
    }

    /// <summary>
    /// تخزين الراتب المتراكم للشهر الحالي للطبيب المتعاقد
    /// يتم استدعاؤها عند تغيير الدور قبل انتهاء الشهر
    /// </summary>
    private async Task ArchiveCurrentMonthSalaryForContractedAsync(Guid doctorId, ContractedRole contractedRole)
    {
        var now = DateTime.Now;

        // تحقق إذا كان هناك راتب مؤرشف للشهر الحالي (تجنب التخزين المكرر)
        var existingRecord = contractedRole.GetSalaryForMonth(now.Year, now.Month);
        if (existingRecord != null)
            return; // تم التخزين مسبقاً

        // احسب راتب الشهر الحالي من بداية الشهر لحد الآن
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var treatments = await _treatmentRepository.GetByDoctorAndPeriodAsync(doctorId, monthStart, now);
        var totalCost = treatments.Sum(t => t.Cost);
        var salary = totalCost * contractedRole.Percent;

        // خزّن الراتب المتراكم للشهر الحالي
        contractedRole.ArchiveCurrentSalary(salary);
    }
    #endregion

    #region Background Methods

    /// <summary>
    /// إغلاق راتب المتعاقدين للشهر الماضي وتخزينه
    /// يتم استدعاء هذه الدالة في أول يوم من كل شهر
    /// </summary>
    public async Task ArchivePreviousMonthSalariesAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var config = await _configRepository.GetAsync();
        bool changed = false;

        foreach (var doctor in doctors)
        {
            if (doctor.ActiveRole is ContractedRole contractedRole)
            {
                var now = DateTime.Now;
                var previousMonth = now.AddMonths(-1);

                // تحقق إذا كان هناك راتب مؤرشف للشهر الماضي (تجنب التخزين المكرر)
                var existingRecord = contractedRole.GetSalaryForMonth(previousMonth.Year, previousMonth.Month);
                if (existingRecord != null)
                    continue;

                // احسب راتب الشهر الماضي
                var monthStart = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var treatments = await _treatmentRepository.GetByDoctorAndPeriodAsync(doctor.Id, monthStart, monthEnd);
                var totalCost = treatments.Sum(t => t.Cost);
                var salary = totalCost * contractedRole.Percent;

                // خزّن الراتب الفعلي (وليس النسبة)
                contractedRole.ArchiveCurrentSalary(salary);
                changed = true;
            }
        }

        if (changed)
        {
            foreach (var doc in doctors)
                await _doctorRepository.UpdateAsync(doc);
        }
    }

    public async Task UpdateTraineeSalariesAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var config = await _configRepository.GetAsync();
        bool changed = false;

        foreach (var doctor in doctors)
        {
            // إذا كان الدور الحالي متدرباً ونشطاً
            if (doctor.ActiveRole is TraineeRole trainee)
            {
                var currentCalculatedSalary = trainee.CalculateSalary(config.BaseSalary);

                // جلب آخر راتب تم أرشفته لهذا الدور
                var lastArchivedSalary = trainee.SalaryHistory.LastOrDefault()?.Amount;

                // إذا اختلف الراتب (بسبب زيادة سنوات الخدمة أو تغير الراتب الأساسي في Config)
                if (lastArchivedSalary != currentCalculatedSalary)
                {
                    trainee.ArchiveCurrentSalary(currentCalculatedSalary);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            foreach (var doc in doctors)
                await _doctorRepository.UpdateAsync(doc);
        }
    }
    #endregion
}