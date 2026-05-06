using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TreatmentController : ControllerBase
{
    private readonly ITreatmentService _treatmentService;

    public TreatmentController(ITreatmentService treatmentService)
    {
        _treatmentService = treatmentService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _treatmentService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _treatmentService.GetByIdAsync(id);
        if (!result.Success) 
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var result = await _treatmentService.GetByPatientIdAsync(patientId);
        return Ok(result.Value);
    }

    [HttpPost("internal")]
    public async Task<IActionResult> CreateInternal([FromBody] CreateTreatmentInternalDto dto)
    {
        var result = await _treatmentService.CreateInternalAsync(dto);
        if (!result.Success) return BadRequest(result.Errors);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("external")]
    public async Task<IActionResult> CreateExternal([FromBody] CreateTreatmentExternalDto dto)
    {
        var result = await _treatmentService.CreateExternalAsync(dto);
        if (!result.Success) return BadRequest(result.Errors);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("assign-doctor")]
    public async Task<IActionResult> AssignDoctor([FromBody] AssignDoctorDto dto)
    {
        var result = await _treatmentService.AssignDoctorAsync(dto);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok();
    }

    [HttpPost("{id:guid}/discharge")]
    public async Task<IActionResult> Discharge(Guid id, [FromBody] DateTime dischargeDate)
    {
        var result = await _treatmentService.DischargeAsync(id, dischargeDate);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _treatmentService.DeleteAsync(id);
        if (!result.Success) return NotFound(result.Errors);
        return NoContent();
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDateRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        var result = await _treatmentService.GetByDateRangeAsync(startDate, endDate);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    [HttpGet("by-doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId)
    {
        var result = await _treatmentService.GetByDoctorAsync(doctorId);
        if (!result.Success) return NotFound(result.Errors);
        return Ok(result.Value);
    }

    [HttpGet("by-doctor/{doctorId:guid}/period")]
    public async Task<IActionResult> GetByDoctorAndPeriod(
    Guid doctorId,
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        var result = await _treatmentService.GetByDoctorAndPeriodAsync(
            doctorId, startDate, endDate);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok(result.Value);
    }
}
