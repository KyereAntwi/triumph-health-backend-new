namespace Triumph.HealthMs.Commands.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<TResult>(this BaseResponse<TResult> result, string routeName = "",
        object? routeValues = null)
    {
        if (!result.IsSuccess)
        {
            return result.Status switch
            {
                404 => Results.NotFound(result),
                403 => Results.Forbid(),
                409 => Results.Conflict(result),
                400 => Results.BadRequest(result),
                401 => Results.Unauthorized(),
                _ => Results.UnprocessableEntity(result)
            };
        }

        return result.Status switch
        {
            204 => Results.NoContent(),
            _ => Results.Ok(result)
        };
    }
}