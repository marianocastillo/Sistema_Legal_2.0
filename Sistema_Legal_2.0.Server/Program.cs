using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
<<<<<<< HEAD


using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2.Server.Providers;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

using Microsoft.AspNetCore.Authentication;
=======
>>>>>>> Developer-Fronk
using Sistema_Legal_2._0.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Providers;
using Microsoft.OpenApi.Models;
using Sistema_Legal_2._0.Server.Entities;
//using Microsoft.CodeAnalysis.Elfie.Diagnostics;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173" , "https://localhost:5174") // Cambia esto si tu frontend tiene otro puerto
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Si usas autenticación con cookies o tokens
        });
});




builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<db_silegContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sistema_Legal"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IUserAccessor>(provider => {
    IHttpContextAccessor context = provider.GetService<IHttpContextAccessor>();
    int uid = Convert.ToInt32(context.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "idUsuario")?.Value);

    return new UserAccessor
    {
        idUsuario = uid
    };
});

builder.Services.AddScoped<Logger>();

// Configuración de Autenticación con JwtBearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
    };
});

builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options => {
    options.InvalidModelStateResponseFactory = (errorContext) =>
    {
        Dictionary<string, string> Errors = new Dictionary<string, string>();

        foreach (var modelStateEntry in errorContext.ModelState)
        {
            foreach (var error in modelStateEntry.Value.Errors)
            {
                Errors.Add(modelStateEntry.Key, error.ErrorMessage);
            }
        }

        return new BadRequestObjectResult(new OperationResult(false, "Los datos ingresados no son válidos", Errors));
    };
});

builder.Services.AddSingleton<ActiveDirectoryAuthenticationService>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<Authentication>();
builder.Services.AddScoped<OnlineUser>();

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
