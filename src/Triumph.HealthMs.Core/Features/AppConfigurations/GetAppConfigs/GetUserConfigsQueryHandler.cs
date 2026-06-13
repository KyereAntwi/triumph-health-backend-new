namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetUserConfigsQueryHandler(
    IApplicationUserManagementDbContext appUserDbContext)
    : IQueryHandler<object, UserInformationDto>
{
    public async Task<BaseResponse<UserInformationDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;

        var userQuery = await appUserDbContext
            .ApplicationUsers
            .Where(u => u.UserId == ctx.UserId)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.OtherNames,
                u.LastName,
                u.ProfileImageUrl,
                u.Email,
                u.Title
            }).FirstOrDefaultAsync(cancellationToken);

        var result = new UserInformationDto(
            userQuery!.Title,
            userQuery.FirstName,
            userQuery.OtherNames,
            userQuery.LastName,
            userQuery.ProfileImageUrl ?? string.Empty,
            userQuery.Email ?? string.Empty);

        return new BaseResponse<UserInformationDto>
        {
            IsSuccess = true,
            Data = result
        };
    }
}