//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//app.UseDefaultFiles();
//app.UseStaticFiles();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.MapFallbackToFile("/index.html");

//app.Run();


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2.Server.Providers;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Sistema_Legal_2._0.Server.Models;


System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<db_silegContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sistema_Legal_2"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IUserAccessor>(provider =>
{
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

builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
{
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

        return new BadRequestObjectResult(new Sistema_Legal_2._0.Server.Models.OperationResult(false, "Los datos ingresados no son válidos", Errors));
    };
});

builder.Services.AddScoped<ActiveDirectoryAuthenticationService>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddScoped<OnlineUser>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
