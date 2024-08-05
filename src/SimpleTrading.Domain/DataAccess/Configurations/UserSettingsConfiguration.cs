using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.DataAccess.Configurations;

internal class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.SelectedProfile)
            .WithOne()
            .HasForeignKey<UserSettings>(x => x.SelectedProfileId)
            .IsRequired();

        builder
            .Navigation(x => x.SelectedProfile)
            .AutoInclude();

        builder
            .Property(x => x.Culture)
            .HasMaxLength(100);

        builder
            .Property(x => x.Language)
            .HasMaxLength(100);

        builder
            .Property(x => x.TimeZone)
            .HasMaxLength(100);
    }
}