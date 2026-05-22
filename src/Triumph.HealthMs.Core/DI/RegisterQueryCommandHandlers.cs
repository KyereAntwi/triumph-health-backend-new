namespace Triumph.HealthMs.Core.DI;

public static class RegisterQueryCommandHandlers
{
    public static IServiceCollection AddQueryCommandHandlers(this IServiceCollection services)
    {
        #region ApplicationUserManagement
        services.AddScoped<ICommandHandler<AddAUserAccountCommand, Guid>, AddAUserAccountCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserInformationCommand, string>, UpdateUserInformationCommandHandler>();
        services
            .AddScoped<ICommandHandler<LinkUserToExistingAccountCommand, Guid>,
                LinkUserToExistingAccountCommandHandler>();
        #endregion

        #region TenantManagment
        services.AddScoped<ICommandHandler<AddTenantAccountCommand, AddTenantAccountResponse>, AddTenantAccountCommandHandler>();
        services.AddScoped<ICommandHandler<RenewSubscriptionCommand, Guid>, RenewSubscriptionCommandHandler>();
        services.AddScoped<ICommandHandler<AddTenantManagerCommand, Guid>, AddTenantManagerCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveTenantManagerCommand, string>, RemoveTenantManagerCommandHandler>();
        services.AddScoped<IQueryHandler<GetTenantsQuery, IEnumerable<TenantDto>>, GetTenantsQueryHandler>();
        #endregion

        #region FacilityManagement
        services.AddScoped<ICommandHandler<AddFacilityCommand, Guid>, AddFacilityCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateFacilityCommand, string>, UpdateFacilityCommandHandler>();
        services.AddScoped<ICommandHandler<AddFacilityManagerCommand, string>, AddFacilityManagerCommandHandler>();
        services
            .AddScoped<ICommandHandler<RemoveFacilityManagerCommand, string>, RemoveFacilityManagerCommandHandler>();
        services
            .AddScoped<IQueryHandler<GetTenantFacilitiesQuery, IEnumerable<TenantFacilityDto>>,
                GetTenantFacilitiesQueryHandler>();
        #endregion

        #region EmployeeManagement
        services.AddScoped<ICommandHandler<AddAnEmployeeCommand, Guid>, AddAnEmployeeCommandHandler>();
        services
            .AddScoped<ICommandHandler<UpdateEmployeePermissionsCommand, string>,
                UpdateEmployeePermissionsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateEmployeeRoleCommand, string>, UpdateEmployeeRoleCommandHandler>();
        services
            .AddScoped<IQueryHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>, GetAllEmployeesQueryHandler>();
        #endregion

        #region PatientManagement
        services.AddScoped<ICommandHandler<AddPatientCommand, Guid>, AddPatientCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePatientCommand, string>, UpdatePatientCommandHandler>();
        services.AddScoped<ICommandHandler<AddPatientIdentityCommand, Guid>, AddPatientIdentityCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveIdentificationCommand, string>, RemoveIdentificationCommandHandler>();
        services.AddScoped<ICommandHandler<AddVisitCommand, Guid>, AddVisitCommandHandler>();
        services.AddScoped<ICommandHandler<TakeVitalMeasurementCommand, string>, TakeVitalMeasurementCommandHandler>();
        #endregion

        #region GeneralManagement
        services.AddScoped<IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>, GetAllPermissionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllSubscriptionsQuery, IEnumerable<SubscriptionDto>>, GetAllSubscriptionsQueryHandler>();
        services.AddScoped<ICommandHandler<AddADrugCommand, Guid>, AddADrugCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllDrugsQuery, IEnumerable<DrugDto>>, GetAllDrugsQueryHandler>();
        services.AddScoped<ICommandHandler<AddHealthDiagnosisCommand, Guid>, AddHealthDiagnosisCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllHealthDiagnosisQuery, IEnumerable<HealthDiagnosisDto>>, GetAllHealthDiagnosisQueryHandler>();
        services.AddScoped<ICommandHandler<AddLabTestCommand, Guid>, AddLabTestCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllLabTestsQuery, IEnumerable<LabTestDto>>, GetAllLabTestsQueryHandler>();
        services.AddScoped<ICommandHandler<AddVitalItemCommand, Guid>, AddVitalItemCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllVitalsQuery, IEnumerable<VitalItemDto>>, GetAllVitalsQueryHandler>();
        #endregion
        
        return services;
    }
}