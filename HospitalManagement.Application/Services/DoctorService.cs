using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Doctors;
using HospitalManagement.Application.Validators.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IValidator<CreateDoctorDto> _createValidator;
    private readonly IValidator<UpdateDoctorDto> _updateValidator;
    private readonly INumberGenerator _numberGenerator;
    private readonly IMapper _mapper;
    public DoctorService(
        IDoctorRepository doctorRepository,
        IValidator<CreateDoctorDto> createValidator,
        IValidator<UpdateDoctorDto> updateValidator,
        INumberGenerator numberGenerator,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _numberGenerator = numberGenerator;
        _mapper = mapper;
    }

    public async Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto)
    {
        try
        {
            //1. validation
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return Result<DoctorDto>.Failure(
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    );
            }

            // 2. check if doctor is exist
            var existingDoctor = await _doctorRepository.ExistAsync(dto.Name , dto.DateOfBirth);

            if (existingDoctor)
            {
                return Result<DoctorDto>.Failure("Doctor already exists.");
            }

            //3. Generate doctor number
            var doctorNumber = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Doctor);

            //4. Create doctor (dtp to entity)
            var doctor = dto.ToEntity(doctorNumber);

            //5. Save to database
            await _doctorRepository.AddAsync(doctor);

            //6. convert to dto ( entity to dto)
            var doctorDto = _mapper.Map<DoctorDto>(doctor);

            return Result<DoctorDto>.SuccessResult(doctorDto);
        }
        catch (Exception ex)
        {
            return Result<DoctorDto>.Failure($"An error occurred while creating the doctor: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
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
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An error occurred while deleting the doctor: {ex.Message}");
        }
    }

    public async Task<Result<List<DoctorDto>>> GetAllAsync()
    {
        try
        {
            var doctors = await _doctorRepository.GetAllAsync();
            var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);
            return Result<List<DoctorDto>>.SuccessResult(doctorDtos);
        }
        catch (Exception ex)
        {
            return Result<List<DoctorDto>>.Failure($"An error occurred while retrieving doctors: {ex.Message}");
        }
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
        Enum.TryParse(dto.Specialization, out Specialization specialization);
        // Update doctor properties
        doctor.UpdateInfo(dto.Name,specialization, dto.DateOfBirth, dto.Address, dto.PhoneNumber, dto.Email);
        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }
}