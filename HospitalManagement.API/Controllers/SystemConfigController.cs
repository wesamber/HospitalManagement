using HospitalManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemConfigController : ControllerBase
{
    private readonly ISystemConfigService _service;

    public SystemConfigController(ISystemConfigService service)
        => _service = service;

    [HttpGet("salary")]
    public async Task<IActionResult> GetBaseSalary()
    {
        var result = await _service.GetBaseSalaryAsync();
        return Ok(new { baseSalary = result.Value });
    }

    [HttpPut("salary")]
    public async Task<IActionResult> UpdateBaseSalary([FromBody] decimal newSalary)
    {
        var result = await _service.UpdateBaseSalaryAsync(newSalary);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok();
    }
}