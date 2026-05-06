using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Departments;
using HospitalManagement.Domain.Entities.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IValidator<CreateDepartmentDto > _createValidator;
    private readonly IValidator<UpdateDepartmentDto> _updateValidator;
    private readonly IMapper _mapper;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IValidator<CreateDepartmentDto> createValidator,
        IValidator<UpdateDepartmentDto> updateValidator,
        IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }
    public async Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if(!validation.IsValid)
            return Result<DepartmentDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var exists = await _departmentRepository.ExistsAsync(dto.Name);
        if (exists)
            return Result<DepartmentDto>.Failure(new List<string> { "Department already exists." });

        var department = dto.ToEntity();
        await _departmentRepository.AddAsync(department);

        var departmentDto = _mapper.Map<DepartmentDto>(department);
        return Result<DepartmentDto>.SuccessResult(departmentDto);
    }
    public async Task<Result<DepartmentDetailsDto>> GetByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return Result<DepartmentDetailsDto>.Failure("Department not found.");

        var dto = await BuildDepartmentDtoAsync(department);
        return Result<DepartmentDetailsDto>.SuccessResult(dto);
    }
    public async Task<Result<List<DepartmentDto>>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();

        var dtos = _mapper.Map<List<DepartmentDto>>(departments);

        return Result<List<DepartmentDto>>.SuccessResult(dtos);
    }

    public async Task<Result<bool>> UpdateAsync(UpdateDepartmentDto dto)
    {
        var department = await _departmentRepository.GetByIdAsync(dto.Id);
        if(department == null)
            return Result<bool>.Failure("Department not found.");

        var validation = _updateValidator.Validate(dto);
        if(!validation.IsValid)
            return Result<bool>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        department.UpdateName(dto.Name);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }
    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return Result<bool>.Failure("Department not found.");

        await _departmentRepository.DeleteAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> AdmitPatientAsync(Guid departmentId, Guid patientId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            return Result<bool>.Failure("Department not found.");

        department.AdmitPatient(patientId);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> RemovePatientAsync(Guid departmentId, Guid patientId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            return Result<bool>.Failure("Department not found.");

        department.RemovePatient(patientId);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> AssignDoctorAsync(Guid departmentId, Guid doctorId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            return Result<bool>.Failure("Department not found.");

        department.AssignDoctor(doctorId);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> RemoveDoctorAsync(Guid departmentId, Guid doctorId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            return Result<bool>.Failure("Department not found.");

        department.RemoveDoctor(doctorId);
        await _departmentRepository.UpdateAsync(department);
        return Result<bool>.SuccessResult(true);
    }

    private async Task<DepartmentDetailsDto> BuildDepartmentDtoAsync(Department department)
    {
        // جيب الدكاترة
        var doctors = new List<DepartmentDoctorDto>();
        foreach (var doctorId in department.DoctorIds)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor != null)
                doctors.Add(new DepartmentDoctorDto
                {
                    Id = doctor.Id,
                    Name = doctor.Name,
                    Specialization = doctor.Specialization.ToString(),
                    DoctorNumber = doctor.DoctorNumber
                });
        }

        // جيب المرضى
        var patients = new List<DepartmentPatientDto>();
        foreach (var patientId in department.PatientIds)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient != null)
                patients.Add(new DepartmentPatientDto
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    PatientNumber = patient.PatientNumber
                });
        }

        return new DepartmentDetailsDto
        {
            Id = department.Id,
            Name = department.Name,
            Doctors = doctors,
            Patients = patients
        };
    }
}
