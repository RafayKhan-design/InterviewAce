using FluentValidation;
using FluentValidation.AspNetCore;
using InterviewAce.API.Middleware;
using InterviewAce.Application.Configurations;
using InterviewAce.Application.DTOs.Common;
using InterviewAce.Application.Interfaces;
using InterviewAce.Application.Interfaces.AI;
using InterviewAce.Application.Interfaces.Authentication;
using InterviewAce.Application.Interfaces.Extraction;
using InterviewAce.Application.Interfaces.JobDescription;
using InterviewAce.Application.Interfaces.Persistence;
using InterviewAce.Application.Interfaces.Resume;
using InterviewAce.Application.Interfaces.ResumeAnalysis;
using InterviewAce.Application.Interfaces.Storage;
using InterviewAce.Application.Services.Authentication;
using InterviewAce.Application.Services.JobDescription;
using InterviewAce.Application.Services.Profile;
using InterviewAce.Application.Services.Resume;
using InterviewAce.Application.Services.ResumeAnalysis;
using InterviewAce.Application.Validators.Authentication;
using InterviewAce.Infrastructure.Persistence;
using InterviewAce.Infrastructure.Persistence.Repositories;
using InterviewAce.Infrastructure.Services;
using InterviewAce.Infrastructure.Services.AI;
using InterviewAce.Infrastructure.Services.Authentication;
using InterviewAce.Infrastructure.Services.Extraction;
using InterviewAce.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,


            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                ))
        };
    });

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

builder.Services.AddScoped<IResumeRepository, ResumeRepository>();

builder.Services.AddScoped<IResumeAnalysisRepository, ResumeAnalysisRepository>();

builder.Services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();

builder.Services.AddScoped<IResumeService, ResumeService>();

builder.Services.AddScoped<IJobDescriptionService, JobDescriptionService>();

builder.Services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();

builder.Services.AddScoped<IResumeAnalyzer, OpenAiResumeAnalyzer>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IResumeTextExtractor, ResumeTextExtractor>();

builder.Services.AddScoped<PdfTextExtractor>();

builder.Services.AddScoped<DocxTextExtractor>();

builder.Services.AddScoped<TxtTextExtractor>();




// Controllers
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                );


            var response = new ApiErrorResponseDto
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            };


            return new BadRequestObjectResult(response);
        };
    });

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssembly(
    typeof(RegisterDtoValidator).Assembly
);

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InterviewAce API",
        Version = "v1",
        Description = "Backend API for the InterviewAce AI Interview Preparation Platform."
    });


    // XML Documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        xmlFile
    );

    options.IncludeXmlComments(xmlPath);


    // JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token.\n\nExample:\nBearer eyJhbGciOiJIUzI1NiIs..."
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();


// HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();