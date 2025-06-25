using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.ToTable("vehiculos");
        builder.HasKey(vehiculo => vehiculo.Id);

        builder.Property(vehiculo => vehiculo.Id)
        .HasConversion(id => id.Value, value => new VehiculoId(value));


        builder.OwnsOne<Direccion>(vehiculo => vehiculo.Direccion);

        builder.Property(vehiculo => vehiculo.Modelo)
        .HasMaxLength(200)
        .HasConversion(vehiculo => vehiculo!.Value, vehiculo => new Modelo(vehiculo));

        builder.Property(vehiculo => vehiculo.Vin)
        .HasMaxLength(500)
        .HasConversion(vehiculo => vehiculo!.Value, vehiculo => new Vin(vehiculo));

        builder.OwnsOne(vehiculo => vehiculo.Precio, priceBuilder =>
        {
            priceBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.OwnsOne(vehiculo => vehiculo.Mantenimiento, priceBuilder =>
        {
            priceBuilder.Property(moneda => moneda.TipoMoneda)
            .HasConversion(TipoMoneda => TipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
        });

        builder.Property<uint>("Version").IsRowVersion();
    }
}
