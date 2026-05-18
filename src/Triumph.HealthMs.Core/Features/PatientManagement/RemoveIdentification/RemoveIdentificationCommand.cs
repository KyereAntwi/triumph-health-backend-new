namespace Triumph.HealthMs.Core.Features.PatientManagement.RemoveIdentification;

public record RemoveIdentificationCommand(
    Guid PatientId,
    Guid IdentificationId);