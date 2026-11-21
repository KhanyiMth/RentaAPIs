
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RentaAPIs.Data;
using RentaAPIs.Models;
using RentaAPIs.Services;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace RentaAPIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //CORS
            var allowedOrigins = builder.Configuration.GetSection("allowedOrigins").Get<string[]>()!;

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(optionsCORS =>
                {
                    optionsCORS.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                });
            });

            //Change where app listens
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
            });

            //Configuring Identity
            builder.Services
                .AddIdentityCore<AppUser>(options =>
                {
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version2;

                    options.Password.RequiredLength = 12;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;

                    options.User.RequireUniqueEmail = true;
                })

                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders(); //For Google Authenticator

            //Add Auth JWT tokens
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("========== JWT AUTH FAILED ==========");
                        Console.WriteLine(context.Exception.Message);
                        Console.WriteLine(context.Exception);
                        Console.WriteLine("=====================================");

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("========== JWT VALIDATED =============");
                        Console.WriteLine("User: " + context.Principal?.Identity?.Name);
                        Console.WriteLine("======================================");

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        Console.WriteLine("========== JWT CHALLENGE =============");
                        Console.WriteLine("Error: " + context.Error);
                        Console.WriteLine("Description: " + context.ErrorDescription);
                        Console.WriteLine("======================================");

                        return Task.CompletedTask;
                    }
                };
        });

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Renta API",
                    Version = "v1",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Token {Bearer} "
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });

            });


            //Register DBContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Registering services
            builder.Services.AddScoped<HashingService>();
            builder.Services.AddScoped<IJwtService, jwtService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<PropertyService>();
            builder.Services.AddScoped<UnitService>();
            builder.Services.AddScoped<TenantService>();
            builder.Services.AddScoped<LeaseService>();
            builder.Services.AddScoped<MaintenanceRequestService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<DocumentService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<AppScoreService>();
            builder.Services.AddScoped<LandlordService>();
            builder.Services.AddScoped<ViewingService>();
            builder.Services.AddScoped<AdvertisementService>();
            builder.Services.AddScoped<TenantApplicationService>();
            builder.Services.AddScoped<RentaMatchService>();
            builder.Services.AddScoped<RentMonitoringService>();
            builder.Services.AddScoped<RentMonitoringBackgroundService>();
            builder.Services.AddScoped<MaintenanceRequestTriageService>();
            builder.Services.AddSingleton<RentaVisionService>();
            builder.Services.AddHostedService<RentMonitoringBackgroundService>();
            builder.Services.AddHttpClient<GeminiLeaseExplanationService>(
                client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(120);
                });

            //Configure Swagger
            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();

            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await SeedData.SeedRolesAsync(services);
            }

            app.UseSwagger();
            app.UseSwaggerUI();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
