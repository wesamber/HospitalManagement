using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using HospitalManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }
    #region Endpoints CRUD Doctor
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _doctorService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _doctorService.GetByIdAsync(id);
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return NotFound(result.Errors);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
    {
        var result = await _doctorService.CreateAsync(dto);
        if(!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id,[FromBody] UpdateDoctorDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID in the URL does not match ID in the body.");
        }
        var result = await _doctorService.UpdateAsync(dto);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _doctorService.DeleteAsync(id);

        if (!result.Success)
            return NotFound(result.Errors);

        return NoContent();
    }
    #endregion

    #region Endpoints GET BY Properties
    [HttpGet("by-specialization/{specialization}")]
    public async Task<IActionResult> GetBySpecialization(string specialization)
    {
        var result = await _doctorService.GetBySpecializationAsync(specialization);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpGet("by-department/{departmentId:guid}")]
    public async Task<IActionResult> GetByDepartment(Guid departmentId)
    {
        var result = await _doctorService.GetByDepartmentAsync(departmentId);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Value);
    }

    [HttpGet("by-number/{doctorNumber}")]
    public async Task<IActionResult> GetByNumber(string doctorNumber)
    {
        var result = await _doctorService.GetByNumberAsync(doctorNumber);
        if (!result.Success) 
            return NotFound(result.Errors);
        return Ok(result.Value);
    }
    #endregion

    #region METHODS DOMAIN

    [HttpPost("{id:guid}/departments/{departmentId:guid}")]
    public async Task<IActionResult> AssignToDepartment(Guid id, Guid departmentId)
    {
        var result = await _doctorService.AssignToDepartmentAsync(id, departmentId);
        if (!result.Success) 
            return BadRequest(result.Errors);
        return Ok();
    }

    [HttpPost("{id:guid}/treatments")]
    public async Task<IActionResult> AddTreatment(Guid id, [FromBody] AddDoctorTreatmentDto dto)
    {
        var result = await _doctorService.AddTreatmentToDoctorAsync(id, dto);
        if (!result.Success)
            return BadRequest(result.Errors);
        return Ok();
    }

    [HttpGet("{id:guid}/salary")]
    public async Task<IActionResult> GetSalary(Guid id)
    {
        var result = await _doctorService.CalculateSalaryAsync(id);
        if (!result.Success)
            return BadRequest(result.Errors);
        return Ok(new { salary = result.Value });
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AddRole(Guid id, [FromBody] AddRoleDoctorDto dto)
    {
        var result = await _doctorService.AddRoleToDoctorAsync(id, dto);
        if (!result.Success)
            return BadRequest(result.Errors);
        return Ok();
    }

    [HttpPost("{id:guid}/promote")]
    public async Task<IActionResult> Promote(Guid id, [FromBody] decimal? baseSalary = null)
    {
        var result = await _doctorService.PromoteDoctorToPermanentAsync(id, baseSalary);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok();
    }
    #endregion
}
