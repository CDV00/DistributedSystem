﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DistributedSystem;
using DistributedSystem.Persistence.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DistributedSystem.Persistence.Configurations;

internal class ActionConfiguration : IEntityTypeConfiguration<Domain.Entities.Identity.Action>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Identity.Action> builder)
    {
        builder.ToTable(TableNames.Actions);
        builder.HasKey(t => t.Id);

        builder.Property(x=>x.Id).HasMaxLength(50);
        builder.Property(x=>x.Name).HasMaxLength(200).IsRequired(true);
        builder.Property(x=>x.IsActive).HasDefaultValue(true);
        builder.Property(x=>x.SortOrder).HasDefaultValue(null);

        //Each User can have many Permission
        builder.HasMany(e=>e.Permissions).WithOne().HasForeignKey(p=>p.ActionId).IsRequired();

        //Each User can have many ActionInFunction
        builder.HasMany(e=>e.ActionInFunctions).WithOne().HasForeignKey(aif=>aif.ActionId).IsRequired();
    }
}
