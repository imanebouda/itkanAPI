global using ITKANSys_api.Models.Entities;
using ITKANSys_api.Core.Interfaces;
using ITKANSys_api.Data.Services;
using ITKANSys_api.Interfaces;
using ITKANSys_api.Services;
using ITKANSys_api.Services.Gestions;
using ITKANSys_api.Services.Param;
using ITKANSys_api.Utility.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Remplacez ceci par l'URL correcte de votre application Angular.
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("local");
    options.UseSqlServer(connectionString);
});




// Add Authentication and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

// Inject app Dependencies (Dependency Injection)

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IProcessusServices, ProcessusService>();
builder.Services.AddScoped<IProcesObjectifsService, ProcesObjectifsService>();
builder.Services.AddScoped<IProcesDocumentsService, ProcesDocumentsService>();

builder.Services.AddScoped<IProceduresService, ProceduresService>();
builder.Services.AddScoped<IProcObjectifsService, ProcObjectifsService>();
builder.Services.AddScoped<IProcDocumentsService, ProcDocumentsService>();

builder.Services.AddScoped<IPQService, PQService>();
builder.Services.AddScoped<IMQService, MQService>();

builder.Services.AddScoped<IIndicateurService, IndicateurService>();
builder.Services.AddScoped<IResultatsIndicateurService, ResultatsIndicateurService>();

builder.Services.AddScoped<IRolesServices,RolesServices>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<IProcessusServices, ProcessusService>();

builder.Services.AddScoped<ITypeOrganismeService,TypeOrganismeService>();
builder.Services.AddScoped<IOrganismeService, OrganismeService>();
builder.Services.AddScoped<IParametrageService, ParametrageService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<ISMQService, SMQService>();
builder.Services.AddScoped<IAuditService,AuditService >();
builder.Services.AddScoped<IConstatService, ConstatService>();


// pipeline

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
