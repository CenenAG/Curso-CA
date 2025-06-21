using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(
        DbContextOptions options, 
        IPublisher publisher,
        ILogger<ApplicationDbContext> logger) : base(options)
    {
        _publisher = publisher;
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            await PublishDomainEventAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("La excepcion por concurrencia en Base de Datos -> " + ex.Message, ex);
        }
        catch (DbUpdateException )
        {
            var errors = new[] { new ValidationError("Database", "Error al actualizar la base de datos. Verifique los datos ingresados.") };
            throw new ValidationException(errors);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
        {
            var errors = new[] { new ValidationError("Database", "Ya existe un registro con los mismos datos únicos.") };
            throw new ValidationException(errors);
        }
    }

    private async Task PublishDomainEventAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.State == EntityState.Added || 
                           entry.State == EntityState.Modified || 
                           entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        if (!domainEvents.Any())
            return;

        _logger.LogInformation("Publishing {Count} domain events", domainEvents.Count);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, CancellationToken.None);
        }

    }
}
