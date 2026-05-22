using Microsoft.EntityFrameworkCore.Design;
using Moq;

namespace Triumph.HealthMs.Persistence.Data.TenantContext;

public class TenantManagementDbContextFactory : IDesignTimeDbContextFactory<TenantManagementDbContext>
{
    public TenantManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantManagementDbContext>();
        
        
        optionsBuilder.UseNpgsql("Host=postgresql-207116-0.cloudclusters.net;Port=10072;Database=TriumphHealthDb;Username=HealthManagmentDbUser;SSL Mode=Require;Password=HealthManagmentDbUserPassword;Trust Server Certificate=true;");

        // Create a mock ILoggedInUserService for design-time
        var mockLoggedInUserService = new Mock<ILoggedInUserService>();
        mockLoggedInUserService.Setup(s => s.UserId).Returns("design_time_user");
        mockLoggedInUserService.Setup(s => s.TenantId).Returns(Guid.Empty.ToString());

        return new TenantManagementDbContext(optionsBuilder.Options, mockLoggedInUserService.Object);
    }
}