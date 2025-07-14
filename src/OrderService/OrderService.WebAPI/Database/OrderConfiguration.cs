using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.WebAPI.Domain;

namespace OrderService.WebAPI.Database;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(d => d.CreatedAt)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(d => d.UpdatedAt)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired()
            .HasColumnName("updated_at");


    }
}
