namespace Triumph.HealthMs.Core.Models.Common;

public record BaseResponse<TEntity>
{
    public int Status { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public TEntity? Data { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
}