using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using UploadImage.API.Interfaces;
using UploadImage.API.Services;
using UploadImage.Database;

var builder = WebApplication.CreateBuilder(args);

// Carregar a configuração do arquivo appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();


// Add services to the container.
var connectionStringDatabase = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UpdateImageDbContext>(options => options.UseSqlServer(connectionStringDatabase)
                                                                            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ContextInitialized)));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<BlobStorageService>(sp =>
{
    var connectionString = configuration.GetConnectionString("BlobServe");
    var containerName = configuration.GetConnectionString("BlobContainerName"); ;
    var imageService = sp.GetService<IImageService>();
    return new BlobStorageService(connectionString, containerName, imageService);
});

builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
