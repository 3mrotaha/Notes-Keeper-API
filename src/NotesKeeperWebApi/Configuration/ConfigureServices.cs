

using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts;
using NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts;
using NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts;
using NotesKeeper.Core.ServiceContracts.JwtServiceContracts;
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;
using NotesKeeper.Core.ServiceContracts.ReminderServiceContracts;
using NotesKeeper.Core.ServiceContracts.TagServiceContracts;
using NotesKeeper.Core.Services;
using NotesKeeper.Core.Services.JwtService;
using NotesKeeper.Infrastructure.ApplicationDbContext;
using NotesKeeper.Infrastructure.Interceptors;
using NotesKeeper.Infrastructure.Repositories;
using NotesKeeperWebApi.Services;
using Swashbuckle.AspNetCore.Filters;

namespace NotesKeeperWebApi.Configuration;

public static class ConfigureServices
{
    public static void AddAndConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            /* Adding the ProducesAttribute and ConsumesAttribute globally to all the controllers and actions to specify that the API will only accept and return JSON data */
            options.Filters.Add(new ProducesAttribute("application/json", "application/pdf"));
            options.Filters.Add(new ConsumesAttribute("application/json"));

            /* Adding the AuthorizeFilter globally to all the controllers and actions to require authentication for all the endpoints by default, this means that all the endpoints will require authentication unless they are decorated with the [AllowAnonymous] attribute */
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                            .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    public static void AddAndConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);

            // reader
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });
    }

    public static void AddAndConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            {
                Title = "Notes Keeper API v1",
                Version = "v1"
            });

            options.SwaggerDoc("v2", new Microsoft.OpenApi.OpenApiInfo
            {
                Title = "Notes Keeper API v2",
                Version = "v2"
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token like: Bearer {token}"
            };

            options.AddSecurityDefinition("Bearer", scheme);

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });

            options.ExampleFilters();
            options.OperationFilter<AddHeaderOperationFilter>("UserId", "User Id", false);
        });

        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    public static void AddAndConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        // adding cors
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularLocalhost", policy =>
            {
                policy.WithOrigins(configuration["Angular:host"] ?? "http://localhost:5200")
                    // .AllowAnyMethod()
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    // .WithHeaders("Content-Type", "Authorization")
                    .AllowAnyHeader();
            });
        });
    }

    public static void AddAndConfigureRateLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            
            options.AddFixedWindowLimiter("fixed", config =>
            {
               config.Window = TimeSpan.FromSeconds(10);
               config.PermitLimit = 60; // 60 req/10s .. 6 req / s
               config.QueueLimit = 100; // 100 requests in queue for next window            
               config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddSlidingWindowLimiter("sliding", config =>
            {
                config.Window = TimeSpan.FromSeconds(30);
                config.SegmentsPerWindow = 3; // window has 3 segments
                config.PermitLimit = 100;
                config.QueueLimit = 10;
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddTokenBucketLimiter("tokenBucket", config =>
            {
                config.TokenLimit = 100;
                config.QueueLimit = 10;
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.AutoReplenishment = true;
                config.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                config.TokensPerPeriod = 20;
            });

            options.AddConcurrencyLimiter("concurrent", config =>
            {
               config.PermitLimit = 10; // allowed only 10 concurrent requests
               config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
               config.QueueLimit = 10; 
            });
        });

        
    }

    public static void AddDbContextAndIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // register interceptors as scoped services so ILogger<T> is injected properly via DI
        services.AddScoped<softDeleteInterceptor>();
        services.AddScoped<DateOfCreationInterceptor>();

        /**
        * injecting the application dbcontext into the DI container
        */
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                /**
                 * assigning the connection string saved in the appsettings.json ConnectionStrings
                 */
                configuration.GetConnectionString("NotesKeeper")
            )
            .AddInterceptors(
                sp.GetRequiredService<softDeleteInterceptor>(),
                sp.GetRequiredService<DateOfCreationInterceptor>()
            );
        });

        // add the identity db context to the DI Container and configure the identity services
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
         {
             options.Password.RequiredLength = 8;
             options.Password.RequireNonAlphanumeric = true;
             options.Password.RequireDigit = true;
             options.Password.RequiredUniqueChars = 3;
             options.Password.RequireLowercase = true;
             options.Password.RequireUppercase = true;

         }).AddEntityFrameworkStores<AppDbContext>()
         .AddDefaultTokenProviders()
         .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
         .AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>();
    }

    public static void AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                string? jwtKey = configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                    throw new ArgumentNullException("Jwt:Key Can't be null or emtpy");

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        services.AddAuthorization();

        // add jwtservice to di container
        services.AddScoped<IJwtService, JwtService>();
    }

    public static void AddApplicationServices(this IServiceCollection services)
    {
        // inject repositories to the DI Container
        services.AddScoped<INoteAddRepository, NoteRepository>();
        services.AddScoped<INoteUpdateRepository, NoteRepository>();
        services.AddScoped<INoteDeleteRepository, NoteRepository>();
        services.AddScoped<INoteGetRepository, NoteRepository>();

        services.AddScoped<ITagAddRepository, TagRepository>();
        services.AddScoped<ITagUpdateRepository, TagRepository>();
        services.AddScoped<ITagDeleteRepository, TagRepository>();
        services.AddScoped<ITagGetRepository, TagRepository>();

        services.AddScoped<IReminderAddRepository, ReminderRepository>();
        services.AddScoped<IReminderUpdateRepository, ReminderRepository>();
        services.AddScoped<IReminderDeleteRepository, ReminderRepository>();
        services.AddScoped<IReminderGetRepository, ReminderRepository>();

        // inject services to the DI Container
        services.AddScoped<INoteAddService, NoteService>();
        services.AddScoped<INoteUpdateService, NoteService>();
        services.AddScoped<INoteDeleteService, NoteService>();
        services.AddScoped<INoteGetService, NoteService>();

        services.AddScoped<ITagAddService, TagService>();
        services.AddScoped<ITagUpdateService, TagService>();
        services.AddScoped<ITagDeleteService, TagService>();
        services.AddScoped<ITagGetService, TagService>();

        services.AddScoped<IReminderAddService, ReminderService>();
        services.AddScoped<IReminderUpdateService, ReminderService>();
        services.AddScoped<IReminderDeleteService, ReminderService>();
        services.AddScoped<IReminderGetService, ReminderService>();
    }

    public static void AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<ReminderNotificationService>();
    }
}