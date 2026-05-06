using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _patientService.GetAllAsync();
        return Ok(result.Value); 
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _patientService.GetByIdAsync(id);
        if (!result.Success) 
            return NotFound(result.Errors);

        return Ok(result.Value); // Internal أو External حسب النوع
    }

    [HttpPost("internal")]
    public async Task<IActionResult> CreateInternal([FromBody] CreatePatientInternalDto dto)
    {
        var result = await _patientService.CreateInternalAsync(dto);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("external")]
    public async Task<IActionResult> CreateExternal([FromBody] CreatePatientExternalDto dto)
    {
        var result = await _patientService.CreateExternalAsync(dto);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        if (id != dto.Id) 
            return BadRequest("ID mismatch.");

        var result = await _patientService.UpdateAsync(dto);
        if (!result.Success) 
            return NotFound(result.Errors);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _patientService.DeleteAsync(id);
        if (!result.Success) 
            return NotFound(result.Errors);

        return NoContent();
    }

    [HttpPost("{id:guid}/admit/{departmentId:guid}")]
    public async Task<IActionResult> Admit(Guid id, Guid departmentId)
    {
        var result = await _patientService.AdmitExternalPatientAsync(id, departmentId);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/discharge")]
    public async Task<IActionResult> Discharge(Guid id)
    {
        var result = await _patientService.DischargeAsync(id);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok();
    }
}
