using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.WebAPI.Domain;

namespace OrderService.WebAPI.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.ComplexProperty(d => d.BoxSize, ib =>
        {
            ib.Property(i => i.Value)
                .IsRequired()
                .HasColumnName("box_size");
        });

        builder.ComplexProperty(o => o.Quantity, ib =>
        {
            ib.Property(i => i.Value)
                .IsRequired()
                .HasColumnName("quantity");
        });

        builder.ComplexProperty(d => d.Period, ib =>
        {
            ib.Property(inner => inner.StartDate)
                .HasConversion(
                    src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                    dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
                .IsRequired()
                .HasColumnName("start_date");

            ib.Property(inner => inner.EndDate)
                .HasConversion(
                    src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                    dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
                .IsRequired()
                .HasColumnName("end_date");
        });
    }
}
