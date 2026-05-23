namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public sealed class GetFacilityConfigsQueryHandler(
    IFacilityManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService) 
    : IQueryHandler<object, FacilityInformationDto>
{
    public async Task<BaseResponse<FacilityInformationDto>> HandleAsync(object query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(loggedInUserService.FacilityId) &&
            string.IsNullOrEmpty(loggedInUserService.FacilityUrlPrefix))
            return new BaseResponse<FacilityInformationDto>
            {
                IsSuccess = false,
                Message = "Facility prefix or facility id is missing"
            };

        var innerQuery = dbContext.OrganizationalFacilities.AsQueryable();

        if (!string.IsNullOrEmpty(loggedInUserService.FacilityId))
        {
            var facility = await innerQuery
                .Where(f => f.Id == Guid.Parse(loggedInUserService.FacilityId))
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
            .Where(f => f.UrlSuffix == loggedInUserService.FacilityUrlPrefix)
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