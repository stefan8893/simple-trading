﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.Configurations;

public class ReferenceConfiguration : IEntityTypeConfiguration<Reference>
{
    public void Configure(EntityTypeBuilder<Reference> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Type)
            .HasMaxLength(100);

        builder
            .Property(x => x.Link)
            .HasMaxLength(4000);

        builder
            .Property(x => x.Notes)
            .HasMaxLength(4000);
    }
}