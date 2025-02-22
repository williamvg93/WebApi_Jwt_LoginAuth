using System.Text;
using ApiJwtAuth.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApiDbContext>(opt => {
    string ?connecStr = builder.Configuration.GetConnectionString("SqlSerConne");
    opt.UseSqlServer(connecStr);
});

// Configuracion de Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(opt => {
    opt.Password.RequireDigit = false; // La contraseña no requiere al menos un número (0-9)
    opt.Password.RequireLowercase = false; // La contraseña No requiere una letra minúscula (a-z)
    opt.Password.RequireUppercase = false; // La contraseña No requiere una letra mayúscula (A-Z)
    opt.Password.RequireNonAlphanumeric = false; // La contraseña No requiere un carácter especial (!@#$%)
    opt.Password.RequiredLength = 1; // Longitud mínima de 1 carácter
    opt.Password.RequiredUniqueChars = 0; // La contraseña No requiere caracteres únicos mínimos

    // Configuración Recomendada para mayor seguridad
    /* opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequiredLength = 8;
    opt.Password.RequiredUniqueChars = 3; */
});

// JWT Configuration
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(optJwtB =>  {
    optJwtB.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => {
        opt.SwaggerEndpoint("/openapi/v1.json", "Auth");
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.Run();

