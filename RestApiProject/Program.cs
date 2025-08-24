using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestApiProject.Profiles;
using RestApiProject.Services;
using Serilog;
using Serilog.Filters;
using System.Text;


//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
//    .WriteTo.Logger(lc => lc
//        .Filter.ByIncludingOnly(Matching.FromSource("RestApiProject.Services.BookService"))
//        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day))
//    .CreateLogger();


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

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//builder.Host.UseSerilog();

//builder.Logging.ClearProviders(); // remove default console logging
//builder.Logging.AddSerilog();     // use only Serilog


string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "book.csv");

// Register BookService with logger injection
builder.Services.AddSingleton<BookService>(sp =>
{
  //  var logger = sp.GetRequiredService<ILogger<BookService>>();
   
    return new BookService( mapper, filePath);
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






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
