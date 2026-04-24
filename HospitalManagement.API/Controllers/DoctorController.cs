using HospitalManagement.Application.DTOs.Doctors;
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

    // http://localhost:7007/api/doctor/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _doctorService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Errors);
    }

    // http://localhost:7001/api/doctor/getById/{id}
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
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        var result = await _doctorService.CreateAsync(dto);
        if(!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
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
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _doctorService.DeleteAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
