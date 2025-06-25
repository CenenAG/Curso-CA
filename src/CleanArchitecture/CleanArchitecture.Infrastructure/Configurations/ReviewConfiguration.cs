using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Reviews;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(review => review.Id);

        builder.Property(review => review.Id)
        .HasConversion(id => id.Value, value => new ReviewId(value));

        builder.Property(review => review.VehiculoId)
        .HasConversion(id => id!.Value, value => new VehiculoId(value));

        builder.Property(review => review.AlquilerId)
        .HasConversion(id => id!.Value, value => new AlquilerId(value));

        builder.Property(review => review.UserId)
        .HasConversion(id => id!.Value, value => new UserId(value));

        builder.Property(review => review.Rating)
        .HasConversion(rating => rating.Value, value => Rating.Create(value).Value);

        builder.Property(review => review.Comentario)
        .HasMaxLength(200)
        .HasConversion(comentario => comentario!.Value, value => new Comentario(value));

        builder.HasOne<Vehiculo>()
        .WithMany()
        .HasForeignKey(review => review.VehiculoId);

        builder.HasOne<Alquiler>()
        .WithMany()
        .HasForeignKey(review => review.AlquilerId);

        builder.HasOne<User>()
        .WithMany()
        .HasForeignKey(review => review.UserId);
    }
}
