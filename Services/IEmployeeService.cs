using MyWebApi.Models;

namespace MyWebApi.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken);
}
