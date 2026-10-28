using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Api.Models;
using EmployeePortal.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeePortal.Api.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<EmployeeResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EmployeeResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetAllAsync(cancellationToken);
        return Ok(employees);
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
