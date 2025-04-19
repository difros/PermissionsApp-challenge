using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PermissionsApp.Domain.Interfaces;
using PermissionsApp.Infraestructure.Persistence;
using PermissionsApp.Infraestructure.Elasticsearch;
using PermissionsApp.Domain.Entities;
using System.Linq;

namespace PermissionsApp.Tests.Integration
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public IntegrationTestBase()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureServices(services =>
                    {
                        // Delete ALL DbContext and SQL Server related records
                        RemoveDbContextRegistration(services);

                        // Adding DbContext with InMemory
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestingDb");
                        }, ServiceLifetime.Scoped);

                        // Replacing external services with test implementations
                        ReplaceExternalServices(services);
                    });
                });

            _client = _factory.CreateClient();
            
            // Create a new scope to get the DbContext
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Initializing the database with test data
            SeedDatabase();
        }

        private void RemoveDbContextRegistration(IServiceCollection services)
        {
            // Remove ALL DbContext and SQL Server related services
            var descriptorsToRemove = services
                .Where(d => 
                    d.ServiceType == typeof(DbContextOptions) || 
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType.Name.Contains("DbContext") ||
                    d.ImplementationType?.Name.Contains("SqlServer") == true ||
                    d.ServiceType.Name.Contains("SqlServer"))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }
        }

        private void ReplaceExternalServices(IServiceCollection services)
        {
            // Replacing Kafka's producer
            var kafkaDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IKafkaProducer));
            if (kafkaDescriptor != null)
            {
                services.Remove(kafkaDescriptor);
            }
            services.AddScoped<IKafkaProducer, TestKafkaProducer>();
            
            // Replace Elasticsearch service
            var elasticsearchDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IElasticsearchService));
            if (elasticsearchDescriptor != null)
            {
                services.Remove(elasticsearchDescriptor);
            }
            services.AddScoped<IElasticsearchService, TestElasticsearchService>();
        }

        private void SeedDatabase()
        {
            try
            {
                // Ensure that the database is created
                _dbContext.Database.EnsureCreated();

                // Clear existing data
                _dbContext.Permissions.RemoveRange(_dbContext.Permissions);
                _dbContext.PermissionTypes.RemoveRange(_dbContext.PermissionTypes);
                _dbContext.SaveChanges();

                // Add test data
                _dbContext.PermissionTypes.AddRange(
                    new PermissionType { Id = 1, Description = "Level 1" },
                    new PermissionType { Id = 2, Description = "Level 2" }
                );

                _dbContext.Permissions.AddRange(
                    new Permission
                    {
                        Id = 1,
                        EmployeeName = "Test",
                        EmployeeLastName = "User",
                        PermissionTypeId = 1,
                        Date = DateTime.Now
                    },
                    new Permission
                    {
                        Id = 2,
                        EmployeeName = "Another",
                        EmployeeLastName = "User",
                        PermissionTypeId = 2,
                        Date = DateTime.Now
                    }
                );
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _scope.Dispose();
            _factory.Dispose();
            _client.Dispose();
        }
    }

    // Test implementation for IKafkaProducer
    public class TestKafkaProducer : IKafkaProducer
    {
        public List<string> ProducedMessages { get; } = new List<string>();

        public Task ProduceAsync(string operationType)
        {
            ProducedMessages.Add(operationType);
            return Task.CompletedTask;
        }
    }
    
    // Test implementation for IElasticsearchService
    public class TestElasticsearchService : IElasticsearchService
    {
        public Task InitializeIndexAsync()
        {
            return Task.CompletedTask;
        }

        public Task IndexDocumentAsync<T>(T document) where T : class
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> SearchAsync<T>(string searchTerm) where T : class
        {
            return Task.FromResult(Enumerable.Empty<T>());
        }

        public Task IndexPermissionAsync(Permission permission)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Permission>> SearchPermissionsAsync(string searchTerm)
        {
            return Task.FromResult(Enumerable.Empty<Permission>());
        }
    }
}