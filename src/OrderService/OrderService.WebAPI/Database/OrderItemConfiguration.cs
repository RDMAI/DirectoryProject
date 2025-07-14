using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.WebAPI.Domain;

namespace OrderService.WebAPI.Database;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.BoxSize)
            .IsRequired()
            .HasColumnName("box_size");

        builder.Property(o => o.Quantity)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(d => d.StartDate)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired()
            .HasColumnName("start_date");

        builder.Property(d => d.EndDate)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired()
            .HasColumnName("end_date");
    }
}
