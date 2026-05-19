namespace Triumph.HealthMs.Queries.QueryTypes;

[ExtendObjectType<QueryBase>]
public class HealthCheckQuery
{
    public string HealthCheck() => $"Server is running";
}