using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Infrastructure.BackgroundJobs;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.Authorization;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.RefreshTokens;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Interceptors;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Seed;
using PharmacyInventoryDispensingSystem.Infrastructure.Services.Email;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var connectionString =configuration.GetConnectionString("DefaultConnection") ??
            throw  new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        ;

        // Register the AppDbContext with the connection string: 
        services.AddDbContext<AppDbContext>((sp,options) => 
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());

        });

        


        //Register Identity:
        services.AddIdentity<ApplicationUser, IdentityRole>(options => 
        {
             //Password Rules:
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            //Lockout settings :

            options.Lockout.MaxFailedAccessAttempts = 5; //N=5
            options.Lockout.AllowedForNewUsers = true;
            //يستمر حظر 15 دقيقة:
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            //User requirements :

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;

        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //Register Jwt Authentication:

        services.Configure<JwtOptions>
          (configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
          .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero

                };


            });

        //Register Authorization Policies:

        services.AddAuthorization(options => 
        {

            // Role-based policies

            options.AddPolicy(
                PolicyNames.AdminOnly,
                policy => policy.RequireRole(RoleNames.Admin));

            options.AddPolicy(
               PolicyNames.ReceptionistOrAdmin,
               policy => policy.RequireRole(RoleNames.Receptionist,
                                            RoleNames.Admin));


            options.AddPolicy(
               PolicyNames.DoctorOrAdmin,
               policy => policy.RequireRole(
                   RoleNames.Doctor,
                   RoleNames.Admin));

            options.AddPolicy(
                PolicyNames.PharmacistOrAdmin,
                policy => policy.RequireRole(
                    RoleNames.Pharmacist,
                    RoleNames.Admin));

            //======================================

            // Permission-based policies

            foreach(var permission in Permissions.All)
            {

                options.AddPolicy(
                 permission,
                  policy =>
                  {
                   policy.RequireAuthenticatedUser();

                  policy.RequireClaim(
                 ApplicationClaimTypes.Permission,
                 permission);
              });

            }


        });
       


        // Email service Register:
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        services.AddTransient<IEmailService, EmailService>();

        services.Configure<AuthenticationOptions>(
        configuration.GetSection(AuthenticationOptions.SectionName));


        //Register Background Services:
        services.AddHostedService<PrescriptionExpirationBackgroundService>();

        //Register Repositries and services:
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IStaffUserService, StaffUserService>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IMedicineRepository, MedicineRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        services.AddScoped<IDispenseRepository, DispenseRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        // Register Interceptor:
        services.AddScoped<ISaveChangesInterceptor,AuditableEntityInterceptor>();
        services.AddScoped<
    IPrescriptionAuthorizationService,
    PrescriptionAuthorizationService>();

        // Current User & Resource Authorization:

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuthorizationHandler, 
            PrescriptionOwnerAuthorizationHandler>();
        services.AddScoped<IUserLookupService, UserLookupService>();


        return services;
    }

    public static async Task InitializeInfrastructureAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        await DatabaseSeeder.SeedIdentityAsync(scope.ServiceProvider, cancellationToken);
        await DatabaseSeeder.SeedCatalogAsync(scope.ServiceProvider, cancellationToken);
    }
}

