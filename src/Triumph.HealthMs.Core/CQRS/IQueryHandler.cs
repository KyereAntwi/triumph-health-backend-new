namespace Triumph.HealthMs.Core.CQRS;

public interface IQueryHandler<in TQuery, TResult>
    where TResult : notnull
{
    Task<BaseResponse<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken =  default);
}