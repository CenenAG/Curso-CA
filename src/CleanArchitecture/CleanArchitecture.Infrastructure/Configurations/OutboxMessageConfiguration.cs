using CleanArchitecture.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutBoxMessage>
{
    public void Configure(EntityTypeBuilder<OutBoxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(outboxMessage => outboxMessage.Id);

        builder.Property(outboxMessage => outboxMessage.Content)
        .HasColumnType("jsonb")
        .IsRequired();

        builder.Property(outboxMessage => outboxMessage.OcurredOnUtc)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.Type)
            .IsRequired();
    }
}