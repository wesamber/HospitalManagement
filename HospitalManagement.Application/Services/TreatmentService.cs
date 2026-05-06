using AutoMapper;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Treatments;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class TreatmentService : ITreatmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly INumberGenerator _numberGenerator;
    private readonly IMapper _mapper;

    public TreatmentService(
        IDepartmentRepository departmentRepository,
        ITreatmentRepository treatmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        INumberGenerator numberGenerator,
        IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _treatmentRepository = treatmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _numberGenerator = numberGenerator;
        _mapper = mapper;
    }

    public async Task<Result<TreatmentInternalDto>> CreateInternalAsync(CreateTreatmentInternalDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
            return Result<TreatmentInternalDto>.Failure("Patient not found.");

        if (patient is not InternalPatient internalPatient) // معالجة داخلية حصرا للمريض الداخلي
            return Result<TreatmentInternalDto>.Failure("Only internal patients can have internal treatments.");

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department == null)
            return Result<TreatmentInternalDto>.Failure("Department not found.");

        var number = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Treatment);

        var treatment = dto.ToDomain(number);

        await _treatmentRepository.AddAsync(treatment);

        internalPatient.AddInternalTreatment(treatment); // ربط المعالجة بالمريض الداخلي
        await _patientRepository.UpdateAsync(internalPatient);

        return Result<TreatmentInternalDto>.SuccessResult(
            _mapper.Map<TreatmentInternalDto>(treatment));
    }

    public async Task<Result<TreatmentExternalDto>> CreateExternalAsync(CreateTreatmentExternalDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
            return Result<TreatmentExternalDto>.Failure("Patient not found.");

        var number = _numberGenerator.GenerateUniqueNumber(NumberPerfix.Treatment);

        var treatment = dto.ToDoamin(number);

        await _treatmentRepository.AddAsync(treatment);

        // ربط المعالجة بالمريض 
        patient.AddExternalTreatment(treatment);
        await _patientRepository.UpdateAsync(patient);

        return Result<TreatmentExternalDto>.SuccessResult(
            _mapper.Map<TreatmentExternalDto>(treatment));
    }

    public async Task<Result<object>> GetByIdAsync(Guid id)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(id);
        if (treatment == null)
            return Result<object>.Failure("Treatment not found.");

        object dto = treatment switch
        {
            TreatmentInternal i => _mapper.Map<TreatmentInternalDto>(i),
            TreatmentExternal e => _mapper.Map<TreatmentExternalDto>(e),
            _ => throw new InvalidOperationException()
        };

        return Result<object>.SuccessResult(dto);
    }
    public async Task<Result<List<TreatmentListDto>>> GetAllAsync()
    {
        var treatments = await _treatmentRepository.GetAllAsync();
        return Result<List<TreatmentListDto>>.SuccessResult(
            _mapper.Map<List<TreatmentListDto>>(treatments));
    }

    public async Task<Result<List<TreatmentListDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var treatments = await _treatmentRepository.GetByPatientIdAsync(patientId);
        return Result<List<TreatmentListDto>>.SuccessResult(
            _mapper.Map<List<TreatmentListDto>>(treatments));
    }

    public async Task<Result<bool>> AssignDoctorAsync(AssignDoctorDto dto)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(dto.TreatmentId);
        if (treatment == null)
            return Result<bool>.Failure("Treatment not found.");

        if (treatment is not TreatmentInternal internalTreatment) // بس المعالجة الداخلية يلي فيها دكاترة
            return Result<bool>.Failure("Only internal treatments can have assigned doctors.");

        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null)
            return Result<bool>.Failure("Doctor not found.");

        var doctorTreatment = new DoctorTreatment(dto.DoctorId, dto.TreatmentId, dto.Role);

        // ربط بالمعالجة
        internalTreatment.AddDoctor(doctorTreatment);
        await _treatmentRepository.UpdateAsync(internalTreatment);

        // ربط بالدكتور
        doctor.AddTreatment(doctorTreatment);
        await _doctorRepository.UpdateAsync(doctor);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> DischargeAsync(Guid treatmentId, DateTime dischargeDate)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(treatmentId);
        if (treatment == null)
            return Result<bool>.Failure("Treatment not found.");

        if (treatment is not TreatmentInternal internalTreatment)
            return Result<bool>.Failure("Only internal treatments can be discharged.");

        internalTreatment.Discharge(dischargeDate);
        await _treatmentRepository.UpdateAsync(internalTreatment);

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(id);
        if (treatment == null)
            return Result<bool>.Failure("Treatment not found.");

        await _treatmentRepository.DeleteAsync(treatment);
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<List<TreatmentListDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return Result<List<TreatmentListDto>>.Failure("Start date cannot be after end date.");

        var treatments = await _treatmentRepository.GetByDateRangeAsync(startDate, endDate);
        return Result<List<TreatmentListDto>>.SuccessResult(
            _mapper.Map<List<TreatmentListDto>>(treatments));
    }

    public async Task<Result<List<TreatmentListDto>>> GetByDoctorAsync(Guid doctorId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<List<TreatmentListDto>>.Failure("Doctor not found.");

        var treatments = await _treatmentRepository.GetByDoctorAsync(doctorId);
        return Result<List<TreatmentListDto>>.SuccessResult(
            _mapper.Map<List<TreatmentListDto>>(treatments));
    }

    public async Task<Result<List<TreatmentListDto>>> GetByDoctorAndPeriodAsync(
    Guid doctorId, DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return Result<List<TreatmentListDto>>.Failure("Start date cannot be after end date.");

        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return Result<List<TreatmentListDto>>.Failure("Doctor not found.");

        var treatments = await _treatmentRepository.GetByDoctorAndPeriodAsync(
            doctorId, startDate, endDate);

        return Result<List<TreatmentListDto>>.SuccessResult(
            _mapper.Map<List<TreatmentListDto>>(treatments));
    }
}
