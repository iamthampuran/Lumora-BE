using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Studio;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.AddEmployees;

public class AddEmployeeCommandHandler(ILogger<AddEmployeeCommandHandler> logger, IUnitOfWork unitOfWork, IGenericRepository<Employee> employeeRepository, 
    IStudioRepository studioRepository)
{
    public async Task<Result<List<Guid>>> Handle(AddEmployeesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AddEmployeesCommand));

        var studio = await studioRepository.GetByIdAsync(command.StudioId, cancellationToken);
        if (studio == null)
        {
            return Result.NotFound("Studio not found");
        }
        var employees = new List<Employee>();
        foreach (var emp in command.EmployeeDetails)
        {
            employees.Add(new Employee() { FullName = emp.Name, EmployeeRole = emp.Role, Email = emp.Email, Phone = emp.Phonenumber, StudioId = command.StudioId });
        }

        employeeRepository.AddRange(employees);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(employees.Select(x => x.Id).ToList());
    }
}
