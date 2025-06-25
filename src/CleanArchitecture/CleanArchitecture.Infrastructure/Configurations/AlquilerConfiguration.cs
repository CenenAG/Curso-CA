using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class AlquilerConfiguration : IEntityTypeConfiguration<Alquiler>
{
    public void Configure(EntityTypeBuilder<Alquiler> builder)
    {
        builder.ToTable("alquileres");
        builder.HasKey(alquiler => alquiler.Id);

        builder.Property(alquiler => alquiler.Id)
        .HasConversion(id => id.Value, value => new AlquilerId(value));

        builder.Property(alquiler => alquiler.VehiculoId)
        .HasConversion(id => id!.Value, value => new VehiculoId(value));

        builder.Property(alquiler => alquiler.UserId)
        .HasConversion(id => id!.Value, value => new UserId(value));

        builder.OwnsOne(alquiler => alquiler.PrecioPorPeriodo, precioBuilder =>
        {
            precioBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.OwnsOne(alquiler => alquiler.PrecioMantenimiento, precioBuilder =>
        {
            precioBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.OwnsOne(alquiler => alquiler.PrecioAccesorios, precioBuilder =>
        {
            precioBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.OwnsOne(alquiler => alquiler.PrecioTotal, precioBuilder =>
        {
            precioBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.OwnsOne(alquiler => alquiler.Duracion);

        builder.HasOne<Vehiculo>()
        .WithMany()
        .HasForeignKey(alquiler => alquiler.VehiculoId);

        builder.HasOne<User>()
        .WithMany()
        .HasForeignKey(alquiler => alquiler.UserId);




    }
}
