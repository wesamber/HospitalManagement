using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Patients;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class PatientService : IPatientService
{

    private readonly IPatientRepository _patientRepository;
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<CreatePatientExternalDto> _createExternalValidator;
    private readonly IValidator<CreatePatientInternalDto> _createInternalValidator;
    private readonly IValidator<UpdatePatientDto> _updateValidator;
    private readonly INumberGenerator _numberGenerator;
    private readonly IMapper _mapper;

    public PatientService(
        IPatientRepository patientRepository,
        ITreatmentRepository treatmentRepository,
        IDepartmentRepository departmentRepository,
        IValidator<CreatePatientExternalDto> createExternalValidator,
        IValidator<CreatePatientInternalDto> createInternalValidator,
        IValidator<UpdatePatientDto> updateValidator,
        INumberGenerator numberGenerator,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _treatmentRepository = treatmentRepository;
        _departmentRepository = departmentRepository;
        _createExternalValidator = createExternalValidator;
        _createInternalValidator = createInternalValidator;
        _updateValidator = updateValidator;
        _numberGenerator = numberGenerator;
        _mapper = mapper;
    }
    #region CRUD Operations
    public async Task<Result<InternalPatientDto>> CreateInternalAsync(CreatePatientInternalDto dto)
    {
        var validation = await _createInternalValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<InternalPatientDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department == null)
            return Result<InternalPatientDto>.Failure("Department not found");

        var existingPatient = await _patientRepository.ExistsAsync(dto.Name, dto.DateOfBirth);
        if (existingPatient)
            return Result<InternalPatientDto>.Failure("Patient already exists");
        var patientNumber = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Patient);

        var patient = dto.ToDomain(patientNumber);
        await _patientRepository.AddAsync(patient);

        department.AdmitPatient(patient.Id);
        await _departmentRepository.UpdateAsync(department);
        return Result<InternalPatientDto>.SuccessResult(_mapper.Map<InternalPatientDto>(patient));
    }

    public async Task<Result<ExternalPatientDto>> CreateExternalAsync(CreatePatientExternalDto dto)
    {
        var validation = await _createExternalValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<ExternalPatientDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var existingPatient = await _patientRepository.ExistsAsync(dto.Name, dto.DateOfBirth);
        if (existingPatient)
            return Result<ExternalPatientDto>.Failure("Patient already exists");

        var patientNumber = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Patient);

        var patient = dto.ToDomain(patientNumber);
        await _patientRepository.AddAsync(patient);

        return Result<ExternalPatientDto>.SuccessResult(_mapper.Map<ExternalPatientDto>(patient));
    }

    public async Task<Result<object>> GetByIdAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            return Result<object>.Failure("Patient not found.");

        var allIds = new List<Guid>();
        if (patient.ExternalTreatmentIds != null)
            allIds.AddRange(patient.ExternalTreatmentIds);

        if (patient is InternalPatient ip && ip.InternalTreatments != null)
            allIds.AddRange(ip.InternalTreatments);

        var treatments = await _treatmentRepository.GetByIdsAsync(allIds);

        var internalTreatmentsDto = _mapper.Map<List<PatientTreatmentDto>>(treatments.OfType<TreatmentInternal>());
        var externalTreatmentsDto = _mapper.Map<List<PatientTreatmentDto>>(treatments.OfType<TreatmentExternal>());

        object dto = patient switch
        {
            InternalPatient i => MapInternalPatient(i, internalTreatmentsDto, externalTreatmentsDto),
            ExternalPatient e => MapExternalPatient(e, externalTreatmentsDto),
            _ => throw new InvalidOperationException("Unknown patient type")
        };

        return Result<object>.SuccessResult(dto);
    }



    public async Task<Result<List<PatientListDto>>> GetAllAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return Result<List<PatientListDto>>.SuccessResult(
            _mapper.Map<List<PatientListDto>>(patients));
    }

    public async Task<Result<bool>> UpdateAsync(UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.Id);
        if (patient == null)
            return Result<bool>.Failure("Patient not found.");

        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<bool>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        patient.UpdateInfo(dto.Name, dto.DateOfBirth, dto.Address, dto.PhoneNumber, dto.Email);
        await _patientRepository.UpdateAsync(patient);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            return Result<bool>.Failure("Patient not found.");

        await _patientRepository.DeleteAsync(patient);
        return Result<bool>.SuccessResult(true);
    }
    #endregion

    #region Domain Actions
    public async Task<Result<InternalPatientDto>> AdmitExternalPatientAsync(Guid patientId, Guid departmentId)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null)
            return Result<InternalPatientDto>.Failure("Patient not found."); 
        
        if (patient is not ExternalPatient externalPatient)
            return Result<InternalPatientDto>.Failure("Patient is already internal.");

        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            return Result<InternalPatientDto>.Failure("Department not found.");
        externalPatient.Admit();

        var internalPatient = externalPatient.ToInternalFromExternal(departmentId);

        await _patientRepository.DeleteAsync(externalPatient);
        await _patientRepository.AddAsync(internalPatient);

        department.AdmitPatient(internalPatient.Id);
        await _departmentRepository.UpdateAsync(department);

        return Result<InternalPatientDto>.SuccessResult(
            _mapper.Map<InternalPatientDto>(internalPatient));
    }

    public async Task<Result<bool>> DischargeAsync(Guid patientId)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null)
            return Result<bool>.Failure("Patient not found.");

        if (patient is not InternalPatient internalPatient)
            return Result<bool>.Failure("Only internal patients can be discharged.");

        if (internalPatient.IsDischarged)
            return Result<bool>.Failure("Patient is already discharged.");

        internalPatient.Discharge(DateTime.Now);
        await _patientRepository.UpdateAsync(internalPatient);

        var department = await _departmentRepository.GetByIdAsync(internalPatient.DepartmentId);
        if (department != null)
        {
            department.RemovePatient(patientId);
            await _departmentRepository.UpdateAsync(department);
        }

        return Result<bool>.SuccessResult(true);
    }
    #endregion

    private InternalPatientDto MapInternalPatient(InternalPatient patient, List<PatientTreatmentDto> internalT, List<PatientTreatmentDto> externalT)
    {
        var mapped = _mapper.Map<InternalPatientDto>(patient);
        mapped.InternalTreatments = internalT;
        mapped.ExternalTreatments = externalT;
        return mapped;
    }

    private ExternalPatientDto MapExternalPatient(ExternalPatient patient, List<PatientTreatmentDto> externalT)
    {
        var mapped = _mapper.Map<ExternalPatientDto>(patient);
        mapped.ExternalTreatments = externalT;
        return mapped;
    }
}
