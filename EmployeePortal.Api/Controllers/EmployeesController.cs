using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Employee_Portal.Application.Interfaces;

namespace EmployeePortal.Api.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    
     [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetAllAsync(
            cancellationToken);

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(
            id, cancellationToken);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(
        Employee_Portal.Application.DTOs.CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.CreateAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.Id },
            employee);
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            ),
            Email = User.FindFirstValue(
                JwtRegisteredClaimNames.Email
            ),
            IsAuthenticated = User.Identity?.IsAuthenticated
        });
    }
}
