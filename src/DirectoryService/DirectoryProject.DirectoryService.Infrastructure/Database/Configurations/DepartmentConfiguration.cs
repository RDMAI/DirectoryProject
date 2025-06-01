﻿using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryProject.DirectoryService.Infrastructure.Database.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => Id<Department>.Create(value))
            .HasColumnName("id");

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasColumnName("is_active");
        builder.HasQueryFilter(d => d.IsActive);  // global query filter

        builder.ComplexProperty(d => d.Name, ib =>
        {
            ib.Property(i => i.Value)
                .IsRequired()
                .HasMaxLength(DepartmentName.MAX_LENGTH)
                .HasColumnName("name");
        });

        //builder.Property(d => d.Name)
        //    .HasConversion(
        //        name => name.Value,
        //        value => DepartmentName.Create(value).Value!)
        //    .IsRequired()
        //    .HasMaxLength(DepartmentName.MAX_LENGTH)
        //    .HasColumnName("name");

        builder.Property(d => d.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? Id<Department>.Create(value.Value) : null)
            .IsRequired(false)
            .HasColumnName("parent_id");

        builder.HasOne(d => d.Parent)
            .WithMany()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);  // restricts deletion of parent if it has any children

        builder.Property(d => d.Path)
            .IsRequired()
            .HasColumnName("path");

        builder.HasIndex(d => d.Path)
            .HasMethod("gist");  // to use ltree's operators

        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(d => d.ChildrenCount)
            .IsRequired()
            .HasColumnName("children_count");

        //builder.Navigation(nameof(Department.DepartmentLocations))
        //    .HasField("_departmentLocations");

        //builder.HasMany("_departmentLocations")
        //    .WithOne(nameof(DepartmentLocation.Department))
        //    .HasForeignKey(nameof(DepartmentLocation.DepartmentId))
        //    .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.DepartmentLocations)
            .WithOne(dl => dl.Department)
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

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

        builder.Property<uint>("version")
            .IsRowVersion();
    }
}
