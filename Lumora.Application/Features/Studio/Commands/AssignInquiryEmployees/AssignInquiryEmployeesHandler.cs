using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Studio;
using Lumora.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.AssignInquiryEmployees;

public class AssignInquiryEmployeesHandler(
    IInquiryRepository inquiryRepository,
    IGenericRepository<Employee> employeeRepository,
    IGenericRepository<InquiryEmployee> inquiryEmployeeRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    ILogger<AssignInquiryEmployeesHandler> logger)
{
    public async Task<Result<Guid>> Handle(AssignInquiryEmployeesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AssignInquiryEmployeesCommand));

        var currentUser = currentUserService.GetCurrentUserDetails();
        if (currentUser == null)
            return Result.Unauthorized("User not logged in");

        if (currentUser.StudioId == null)
            return Result.Forbidden("User does not have studio access");

        var inquiry = await inquiryRepository.GetFirstAsync(
            i => i.Id == command.InquiryId,
            null,
            [i => i.InquiryEmployees, i => i.Event],
            disableTracking: false,
            cancellationToken);

        if (inquiry == null)
            return Result.NotFound("Inquiry not found");

        if (inquiry.StudioId != currentUser.StudioId.Value)
            return Result.Forbidden("You do not have access to this inquiry");

        if (inquiry.Status != InquiryStatus.Confirmed)
            return Result.Error("Employees can be assigned only for confirmed inquiries.");

        var requestedEmployeeIds = command.EmployeeIds.Distinct().ToList();

        var studioEmployees = await employeeRepository.GetAsync(
            e => e.StudioId == currentUser.StudioId.Value && requestedEmployeeIds.Contains(e.Id),
            cancellationToken);

        if (studioEmployees.Count != requestedEmployeeIds.Count)
            return Result.Error("One or more employees are invalid for this studio.");

        var existingAssignments = inquiry.InquiryEmployees.ToList();
        var existingEmployeeIds = existingAssignments.Select(x => x.EmployeeId).ToHashSet();

        var assignmentsToRemove = existingAssignments
            .Where(x => !requestedEmployeeIds.Contains(x.EmployeeId))
            .ToList();

        var assignmentsToAdd = requestedEmployeeIds
            .Where(id => !existingEmployeeIds.Contains(id))
            .Select(id => new InquiryEmployee
            {
                InquiryId = inquiry.Id,
                EmployeeId = id
            })
            .ToList();

        if (assignmentsToRemove.Count > 0)
            inquiryEmployeeRepository.Delete(assignmentsToRemove);

        if (assignmentsToAdd.Count > 0)
            inquiryEmployeeRepository.AddRange(assignmentsToAdd);

        inquiry.Event.Status = EventStatus.InProgress;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(inquiry.Id);
    }
}
