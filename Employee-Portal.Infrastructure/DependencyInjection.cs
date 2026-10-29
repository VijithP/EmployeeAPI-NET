using Employee_Portal.Application.Interfaces;
using Employee_Portal.Infrastructure.Data;
using Employee_Portal.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Employee_Portal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<SqlConnectionFactory>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}