using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PermissionsApp.Application.Mappings;
using PermissionsApp.Domain.Interfaces;
using PermissionsApp.Infraestructure.Elasticsearch;
using PermissionsApp.Infraestructure.Kafka;
using PermissionsApp.Infraestructure.Persistence;
using PermissionsApp.Infraestructure.Repositories;
using System.Text.Json.Serialization;
using Elastic.Clients.Elasticsearch;
using PermissionsApp.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PermissionsApp API", Version = "v1" });
});

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure().MigrationsAssembly("PermissionsApp")
    ));


// Repository & UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Elasticsearch
var elasticsearchSettings = builder.Configuration.GetSection("ElasticsearchSettings");
var elasticsearchUri = elasticsearchSettings["Uri"];
var defaultIndex = elasticsearchSettings["DefaultIndex"];

// New client of Elasticsearch
var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri))
    .DefaultIndex(defaultIndex);

builder.Services.AddSingleton(new ElasticsearchClient(settings));
builder.Services.AddScoped<IElasticsearchService>(provider =>
    new ElasticsearchService(
        provider.GetRequiredService<ElasticsearchClient>(),
        defaultIndex
    )
);

// Kafka
builder.Services.AddSingleton<IKafkaProducer>(provider => new KafkaProducer(
    builder.Configuration["KafkaSettings:BootstrapServers"],
    builder.Configuration["KafkaSettings:Topic"]
));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PermissionsApp API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (!context.PermissionTypes.Any())
        {
            context.PermissionTypes.AddRange(
                new PermissionType { Description = "Level 1" },
                new PermissionType { Description = "Level 2" },
                new PermissionType { Description = "Level 3" }
            );
            context.SaveChanges();
        }

        // Initialize Elasticsearch index if needed
        var elasticsearchService = services.GetRequiredService<IElasticsearchService>();
        await elasticsearchService.InitializeIndexAsync();

    } catch (Exception ex) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();