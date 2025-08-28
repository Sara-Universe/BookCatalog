using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestApiProject.Exceptions;
using RestApiProject.Profiles;
using RestApiProject.Services;
using Serilog;
using Serilog.Filters;
using System.Text;
using System.Text.Json;

//logger configuration and setup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.FromSource("RestApiProject.Services.BookService"))
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Create MapperConfiguration
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<BookProfile>();
});

// Create IMapper instance
IMapper mapper = mapperConfig.CreateMapper();

// Register it as a singleton
builder.Services.AddSingleton(mapper);


// Add services to the container.

builder.Services.AddOpenApi();
//for serilog logger
builder.Host.UseSerilog();
builder.Logging.ClearProviders(); // remove default console logging
builder.Logging.AddSerilog();     // use only Serilog


string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "book.csv");

builder.Services.AddSingleton<CsvBookService>(sp =>
{
    var mapper = sp.GetRequiredService<IMapper>(); // get the singleton mapper
    var logger = sp.GetRequiredService<ILogger<BookService>>();


    return new CsvBookService(mapper, filePath, logger);
});
builder.Services.AddSingleton<BookService>(sp =>
{
    var csvService = sp.GetRequiredService<CsvBookService>();
    var mapperInstance = sp.GetRequiredService<IMapper>();
    var logger = sp.GetRequiredService<ILogger<BookService>>();

    return new BookService(csvService, mapperInstance, logger);
});


// Add JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddSingleton<JWTService>();
builder.Services.AddScoped<UserService>();


builder.Services.AddAuthorization();


builder.Services.AddControllers();





var app = builder.Build();

//global exception handler (middleware)
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        // Determine status code based on exception type
        int statusCode = exception switch
        {
            RestApiProject.Exceptions.NotFoundException => StatusCodes.Status404NotFound,
            RestApiProject.Exceptions.ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            Message = exception?.Message ?? "An unexpected error occurred",
        });

        await context.Response.WriteAsync(result);
    });
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
