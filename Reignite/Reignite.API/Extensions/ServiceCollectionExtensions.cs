using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Application.Options;
using Reignite.Infrastructure.Data;
using Reignite.Infrastructure.Mappings;
using Reignite.Infrastructure.Repositories;
using Reignite.Infrastructure.Services;
using System.Text;

namespace Reignite.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Build connection string from environment variables
            var connectionString = BuildConnectionString();

            // Database
            services.AddDbContext<ReigniteDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Mapster
            services.AddMapster();
            MappingConfig.Configure();

            // Repositories
            services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IHobbyService, HobbyService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IFileStorageService>(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                return new FileStorageService(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"));
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT_SECRET not configured");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "Reignite";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ReigniteApp";

            // Configure JwtSettings for injection
            services.Configure<JwtSettings>(options =>
            {
                options.Secret = jwtSecret;
                options.Issuer = jwtIssuer;
                options.Audience = jwtAudience;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (environment.IsDevelopment())
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    }
                    else
                    {
                        var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',')
                            ?? new[] { "https://yourdomain.com" };

                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                });
            });

            return services;
        }

        private static string BuildConnectionString()
        {
            return $"Server={Environment.GetEnvironmentVariable("DB_SERVER")},{Environment.GetEnvironmentVariable("DB_PORT")};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"User id={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                   "TrustServerCertificate=True;MultipleActiveResultSets=true";
        }
    }
}
