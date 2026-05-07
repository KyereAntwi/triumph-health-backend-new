// Global using directives

global using FluentValidation;
global using MassTransit;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Triumph.HealthMs.Core.CQRS;
global using Triumph.HealthMs.Core.Enums;
global using Triumph.HealthMs.Core.Events;
global using Triumph.HealthMs.Core.Features.ApplicationUser.AddAUserAccount;
global using Triumph.HealthMs.Core.Features.ApplicationUser.UpdateUserInfomation;
global using Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;
global using Triumph.HealthMs.Core.Features.TenantManagement.AddTenantManager;
global using Triumph.HealthMs.Core.Features.TenantManagement.RemoveTenantManager;
global using Triumph.HealthMs.Core.Features.TenantManagement.RenewSubscription;
global using Triumph.HealthMs.Core.Interfaces;
global using Triumph.HealthMs.Core.Models.ApplicationUser;
global using Triumph.HealthMs.Core.Models.Common;
global using Triumph.HealthMs.Core.Models.Tenants;
global using Triumph.HealthMs.Core.Utils;