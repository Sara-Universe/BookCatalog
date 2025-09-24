using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestApiProject.Data;
using RestApiProject.Profiles;
using RestApiProject.Services;
using Serilog;
using Serilog.Filters;
using System.Text;
using System.Text.Json;
using RestApiProject.Data;

//logger configuration and setup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(
    ev => Matching.FromSource("RestApiProject.Services")(ev) ||
          Matching.FromSource("RestApiProject.Controllers")(ev)
)
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();

////////////////////////////////////////////////////////////////////////
///
var builder = WebApplication.CreateBuilder(args);


//////////////////////////////////////////////////////
// Create MapperConfiguration
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<BookProfile>();
});

// Create IMapper instance
IMapper mapper = mapperConfig.CreateMapper();

// Register it as a singleton
builder.Services.AddSingleton(mapper);
/////////////////////////////////////////////////////////////////////////
builder.Services.AddDbContext<MyDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));
// Add services to the container.

builder.Services.AddOpenApi();
//for serilog logger
builder.Host.UseSerilog();
builder.Logging.ClearProviders(); // remove default console logging
builder.Logging.AddSerilog();     // use only Serilog

var userFilePath = Path.Combine(builder.Environment.ContentRootPath, "Data", "user.csv");

// Register CsvUserService as a singleton
builder.Services.AddSingleton<CsvUserService>(sp =>
{
    var mapper = sp.GetRequiredService<AutoMapper.IMapper>();
    var logger = sp.GetRequiredService<ILogger<CsvUserService>>();
    return new CsvUserService(mapper, userFilePath, logger);
});
////////////////////////////////////////////////////////////////////////

string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "book.csv");

builder.Services.AddSingleton<CsvBookService>(sp =>
{
    var mapper = sp.GetRequiredService<IMapper>(); // get the singleton mapper
    var logger = sp.GetRequiredService<ILogger<CsvBookService>>();


    return new CsvBookService(mapper, filePath, logger);
});




string filePathborrow = Path.Combine(Directory.GetCurrentDirectory(), "Data", "history.csv");

builder.Services.AddSingleton<CsvBorrowService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<CsvBorrowService>>();


    return new CsvBorrowService(filePathborrow, logger);
});

////////////////////////////////////////////////////////////////////////


builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<UserLoginService>();

builder.Services.AddSingleton<BorrowService>(sp =>
{
    var bookService = sp.GetRequiredService<CsvBookService>();
    var userService = sp.GetRequiredService<CsvUserService>();
    var borrowService = sp.GetRequiredService<CsvBorrowService>();
    var logger = sp.GetRequiredService<ILogger<BorrowService>>();

    return new BorrowService(bookService, userService, logger, borrowService);
});

////////////////////////////////////////////////////////////////////////
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
///////////////////////////////////////////////////////////////////

builder.Services.AddAuthorization();
builder.Services.AddControllers();


//////////////////////////////////////////////////////////////////


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
//////////////////////////////////////////////////////////////////

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
