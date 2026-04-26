using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Doctors;
using HospitalManagement.Application.Validators.Doctors;
using HospitalManagement.Domain.Entities.Config;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
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
    private readonly IValidator<CreateDoctorDto> _createValidator;
    private readonly IValidator<UpdateDoctorDto> _updateValidator;
    private readonly IValidator<AddRoleDoctorDto> _addRoleValidator;
    private readonly ISystemConfigRepository _configRepository;
    private readonly INumberGenerator _numberGenerator;
    private readonly IMapper _mapper;
    public DoctorService(
        IDoctorRepository doctorRepository,
        ITreatmentRepository treatmentRepository,
        IValidator<CreateDoctorDto> createValidator,
        IValidator<UpdateDoctorDto> updateValidator,
        IValidator<AddRoleDoctorDto> addRoleValidator,
        ISystemConfigRepository systemConfigRepository,
        INumberGenerator numberGenerator,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
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
        if(doctor == null)
            return Result<DoctorDetailsDto>.Failure("Doctor not found.");

        var doctorDetailsDto = _mapper.Map<DoctorDetailsDto>(doctor);

        return Result<DoctorDetailsDto>.SuccessResult(doctorDetailsDto);
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
        doctor.AssignToDepartment(departmentId);
        await _doctorRepository.UpdateAsync(doctor);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> AddTreatmentToDoctorAsync(Guid doctorId , AddDoctorTreatmentDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        //var treatment = _mapper.Map<DoctorTreatment>(dto);
        if (!Enum.TryParse<TreatmentRole>(dto.TreatmentRole, true, out var role))
            return Result<bool>.Failure("Invalid treatment role.");

        var treatment = new DoctorTreatment(doctorId, dto.TreatmentId, role);

        doctor.AddTreatment(treatment);
        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<decimal>> CalculateSalaryAsync(Guid doctorId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<decimal>.Failure("Doctor not found.");

        decimal referenceAmount;

        if (doctor.ActiveRole is ContractedRole)
        {
            var treatmentIds = doctor.Treatments.Select(t => t.TreatmentId).ToList();
            var treatments = await _treatmentRepository.GetByIdsAsync(treatmentIds);
            referenceAmount = treatments.Sum(t => t.Cost);
        }
        else
        {
            var config = await _configRepository.GetAsync();
            referenceAmount = config.BaseSalary;
        }
        var salary = doctor.CalculateSalary(referenceAmount);
        return Result<decimal>.SuccessResult(salary);
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
        decimal amountToArchive = role switch
        {
            PermanentRole p => p.BaseSalary,
            ContractedRole c => c.Percent,
            TraineeRole t => t.CalculateSalary(systemBaseSalary), // بيحسب الـ 50% تلقائياً
            _ => 0
        };

        role.ArchiveCurrentSalary(amountToArchive);
    }
    #endregion

    #region Background Methods
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