// Global using directives

global using System.Net.Http.Json;
global using MassTransit;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Hybrid;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Triumph.HealthMs.Core.Enums;
global using Triumph.HealthMs.Core.Features.EmployeeManagement.AddAnEmployee;
global using Triumph.HealthMs.Core.Features.PatientManagement.AddPatient;
global using Triumph.HealthMs.Core.Features.TenantManagement.AddTenantAccount;
global using Triumph.HealthMs.Core.Interfaces;
global using Triumph.HealthMs.Core.Utils;
global using Triumph.HealthMs.ExternalServices.CachingServices;
global using Triumph.HealthMs.ExternalServices.EventHandlers;
global using Triumph.HealthMs.ExternalServices.MessagingTemplates;
global using Triumph.HealthMs.ExternalServices.Services;