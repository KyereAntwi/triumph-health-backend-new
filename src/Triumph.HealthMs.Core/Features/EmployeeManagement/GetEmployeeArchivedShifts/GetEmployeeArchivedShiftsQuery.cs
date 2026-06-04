namespace Triumph.HealthMs.Core.Features.EmployeeManagement.GetEmployeeArchivedShifts;

public record GetEmployeeArchivedShiftsQuery(
    string EmployeeId,
    string From = "",
    string To = "",
    int Page = 1,
    int PageSize = 10);

public record ArchivedShiftDto(
    string Id,
    string StartedAt,
    string EndedAt,
    string ShiftType,
    string TimeStamp,
    string CreatedBy,
    string DayOfWeek,
    int DurationInHours
);