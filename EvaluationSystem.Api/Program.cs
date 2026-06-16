using AutoMapper;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Mapping;
using EvaluationSystem.Api.MiddleWare;
using EvaluationSystem.Application.Services.AuthServices;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Application.Validators;
using EvaluationSystem.Domain.Models;
using EvaluationSystem.Infrastructure.Data;
using EvaluationSystem.Infrastructure.Repositories;
//using EvaluationSystem.Infrastructure.Seeds;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using EvaluationSystem.Application.Services.Evaluation_Service;
using EvaluationSystem.Application.Services.TemplateServices;
using EvaluationSystem.Application.Services.SectionService;
using System.Text.Json.Serialization;
using EvaluationSystem.Application.Services;
using EvaluationSystem.Application.Services.AssignmentService;
using EvaluationSystem.Application.Services.CriteriaService;
using EvaluationSystem.Application.Services.ReportService;
using EvaluationSystem.Infrastructure.Seeds;

namespace EvaluationSystem.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
                             .WriteTo.Console()
                             .WriteTo.File("Logs/log-.txt",
                                 rollingInterval: RollingInterval.Day));
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEvaluationTemplateService, TemplateService>();
            builder.Services.AddScoped<IGenericRepo<EvaluationTemplate>, GenericRepo<EvaluationTemplate>>();
            builder.Services.AddScoped<IGenericRepo<EvaluationSection>, GenericRepo<EvaluationSection>>();
            builder.Services.AddScoped<IEvaluationCriteriaService, CriteriaService>();
            builder.Services.AddScoped<IEvaluationService, EvaluationService>();
            builder.Services.AddScoped<IEvaluationSectionService, SectionService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<JwtHelper>();

            builder.Services.AddScoped(typeof(IGenericRepo<>),
                                       typeof(GenericRepo<>));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IEvaluationService, EvaluationService>();
            builder.Services.AddScoped<IEvaluationAssignmentService, EvaluationAssignmentService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ScoreCalculator>();
            builder.Services.AddScoped<IEvaluationReportService, ReportService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddAutoMapper(op =>
            {
                op.AddProfile<AuthProfile>();
                op.AddProfile<EvaluationTemplateProfile>();
                op.AddProfile<EvaluationSectionProfile>();
                op.AddProfile<EvaluationCriteriaProfile>();
                op.AddProfile<EvaluationProfile>();
                op.AddProfile<DepartmentProfile>();
                op.AddProfile<UserProfile>();
                op.AddProfile<EvaluationAssignmentProfile>();
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,

                        ValidIssuer =
                            builder.Configuration["JwtSettings:Issuer"],

                        ValidAudience =
                            builder.Configuration["JwtSettings:Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    builder.Configuration["JwtSettings:Key"]!))
                    };
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT Token"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularFrontend",
                    policy => policy.WithOrigins("http://localhost:4200") // The default Angular port
                                    .AllowAnyMethod()
                                    .AllowAnyHeader().AllowCredentials());
            });
            var app = builder.Build();

            await DataSeeder.InitializeAsync(app.Services);

            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularFrontend");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}