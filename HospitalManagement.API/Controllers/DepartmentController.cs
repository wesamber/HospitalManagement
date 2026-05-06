using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _departmentService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _departmentService.GetByIdAsync(id);
        if (!result.Success) 
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
    {
        var result = await _departmentService.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
    {
        if (id != dto.Id) 
            return BadRequest("ID mismatch.");

        var result = await _departmentService.UpdateAsync(dto);
        if (!result.Success)
            return NotFound(result.Errors);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _departmentService.DeleteAsync(id);
        if (!result.Success) 
            return NotFound(result.Errors);

        return NoContent();
    }

    [HttpPost("{id:guid}/patients/{patientId:guid}")]
    public async Task<IActionResult> AdmitPatient(Guid id, Guid patientId)
    {
        var result = await _departmentService.AdmitPatientAsync(id, patientId);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpDelete("{id:guid}/patients/{patientId:guid}")]
    public async Task<IActionResult> RemovePatient(Guid id, Guid patientId)
    {
        var result = await _departmentService.RemovePatientAsync(id, patientId);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("{id:guid}/doctors/{doctorId:guid}")]
    public async Task<IActionResult> AssignDoctor(Guid id, Guid doctorId)
    {
        var result = await _departmentService.AssignDoctorAsync(id, doctorId);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpDelete("{id:guid}/doctors/{doctorId:guid}")]
    public async Task<IActionResult> RemoveDoctor(Guid id, Guid doctorId)
    {
        var result = await _departmentService.RemoveDoctorAsync(id, doctorId);
        if (!result.Success) 
            return BadRequest(result.Errors);

        return Ok();
    }
}