using AutoMapper;
using RestApiProject.Profiles;
using RestApiProject.Services;

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


string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "book.csv");

builder.Services.AddSingleton<BookService>(sp =>
{
    var mapper = sp.GetRequiredService<IMapper>();
    return new BookService(mapper, filePath);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
