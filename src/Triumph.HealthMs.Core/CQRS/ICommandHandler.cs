namespace Triumph.HealthMs.Core.CQRS;

public interface ICommandHandler<in TCommand, TResult>
    where TResult : notnull
    where TCommand : notnull
{
    Task<BaseResponse<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}