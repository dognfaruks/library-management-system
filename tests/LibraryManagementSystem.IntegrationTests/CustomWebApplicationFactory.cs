using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "IntegrationTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Gerçek PostgreSQL DbContext kaydını kaldırıyoruz
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Yerine, testler için izole bir InMemory veritabanı ekliyoruz
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Veritabanını oluştur ve USER/ADMIN rollerini seed et
            // (gerçek veritabanında migration ile geliyordu, InMemory'de elle ekliyoruz)
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            db.Database.EnsureCreated();

            if (!db.Roles.Any())
            {
                db.Roles.Add(new Role { Id = 1, Name = "USER" });
                db.Roles.Add(new Role { Id = 2, Name = "ADMIN" });
                db.SaveChanges();
            }
        });
    }
}