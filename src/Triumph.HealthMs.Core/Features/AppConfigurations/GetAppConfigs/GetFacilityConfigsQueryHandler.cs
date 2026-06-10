namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetFacilityConfigsQueryHandler(
    IFacilityManagementDbContext dbContext)
    : IQueryHandler<object, FacilityInformationDto>
{
    public async Task<BaseResponse<FacilityInformationDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        var ctx = (AppConfigUserContext)query;
        if (string.IsNullOrEmpty(ctx.FacilityId) &&
            string.IsNullOrEmpty(ctx.FacilityUrlPrefix))
            return new BaseResponse<FacilityInformationDto>
            {
                IsSuccess = false,
                Message = "Facility prefix or facility id is missing"
            };

        var innerQuery = dbContext.OrganizationalFacilities.AsQueryable();

        if (!string.IsNullOrEmpty(ctx.FacilityId))
        {
            var facility = await innerQuery
                .Where(f => f.Id == Guid.Parse(ctx.FacilityId))
                .Select(f => new FacilityInformationDto(
                    f.Id.ToString(),
                    f.Name,
                    f.LogoUrl ?? string.Empty,
                    f.Address))
                .FirstAsync(cancellationToken);

            return new BaseResponse<FacilityInformationDto>
            {
                IsSuccess = true,
                Data = facility
            };
        }

        var result = await innerQuery
            .Where(f => f.UrlSuffix == ctx.FacilityUrlPrefix)
            .Select(f => new FacilityInformationDto(
                f.Id.ToString(),
                f.Name,
                f.LogoUrl ?? string.Empty,
                f.Address))
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return new BaseResponse<FacilityInformationDto>
            {
                IsSuccess = false,
                Message = "Facility not found"
            };
        }

        return new BaseResponse<FacilityInformationDto>
        {
            IsSuccess = true,
            Data = result
        };
    }
}