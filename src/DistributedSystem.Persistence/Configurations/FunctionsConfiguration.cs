﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DistributedSystem.Persistence.Constants;

namespace DistributedSystem.Persistence.Configurations;

internal sealed class FunctionsConfiguration : IEntityTypeConfiguration<Domain.Entities.Identity.Function>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Identity.Function> builder)
    {
        builder.ToTable(TableNames.Functions);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired(true);
        builder.Property(x => x.ParrentId)
            .HasMaxLength(50)
            .HasDefaultValue(null);
        builder.Property(x => x.CssClass).HasMaxLength(50).HasDefaultValue(null);
        builder.Property(x => x.Url).HasMaxLength(50).IsRequired(true);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.SortOrder).HasDefaultValue(null);

        // Each User can have many Permission
        builder.HasMany(e => e.Permissions)
            .WithOne()
            .HasForeignKey(p => p.FunctionId)
            .IsRequired();

        // Each User can have many ActionInFunction
        builder.HasMany(e => e.ActionInFunctions)
            .WithOne()
            .HasForeignKey(aif => aif.FunctionId)
            .IsRequired();
    }
}
