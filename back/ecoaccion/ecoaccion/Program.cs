using ecoaccion.Application.Mapper;
using ecoaccion.Application.Services.Admin;
using ecoaccion.Application.Services.Auth;
using ecoaccion.Application.Services.Desafios;
using ecoaccion.Application.Services.Interacciones;
using ecoaccion.Application.Services.Participaciones;
using ecoaccion.Application.Services.User;
using ecoaccion.Application.Validator.Auth;
using ecoaccion.Application.Validator.Desafios;
using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Admin;
using ecoaccion.Core.Interfaces.Services.Auth;
using ecoaccion.Core.Interfaces.Services.Desafios;
using ecoaccion.Core.Interfaces.Services.Interacciones;
using ecoaccion.Core.Interfaces.Services.Participaciones;
using ecoaccion.Core.Interfaces.Services.User;
using ecoaccion.Infrastructure.Persistence;
using ecoaccion.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================= CORS =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    //  NO usar .AllowCredentials() con Bearer; no hace falta
    );
});

// ======================= JWT =======================
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
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

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "No estás autenticado. Inicia sesión para continuar."
            });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "No tienes permiso para acceder a este recurso."
            });
            return context.Response.WriteAsync(result);
        }
    };
});

// ======================= SERVICES =======================
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddKeyedScoped<IAdminService, AdminService>("adminService");
builder.Services.AddKeyedScoped<IUserService, UserService>("userService");
builder.Services.AddKeyedScoped<IDesafioService, DesafioService>("desafioService");
builder.Services.AddKeyedScoped<IinteraccionService, InteraccionService>("interaccionService");
builder.Services.AddKeyedScoped<IParticipacionService, ParticipacionService>("participacionService");

builder.Services.AddScoped<JwtService>();

builder.Services.AddKeyedScoped<IAdminRepository, AdminRepository>("adminRepository");
builder.Services.AddKeyedScoped<IUserRepository, UserRepository>("userRepository");
builder.Services.AddKeyedScoped<IDesafioRepository, DesafioRepository>("desafioRepository");
builder.Services.AddKeyedScoped<IinteraccionRepository, InteraccionRepository>("interaccionRepository");
builder.Services.AddKeyedScoped<IParticipacionRepository, ParticipacionRepository>("participacionRepository");

builder.Services.AddScoped<IValidator<AdminInsertDto>, AdminValidator>();
builder.Services.AddScoped<IValidator<UserInsertDto>, UserInsertValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateValidator>();
builder.Services.AddScoped<IValidator<DesafioInsertDto>, DesafioInsertValidator>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();

// ======================= DB =======================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// ======================= API / Swagger =======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ecoaccion API",
        Version = "v1",
        Description = "API para gestionar desafios ecológicos",
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Introduce tu token JWT aquí (sin 'Bearer ')",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ======================= Pipeline =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚠️ Orden correcto: CORS → Auth → AuthZ → MapControllers
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
