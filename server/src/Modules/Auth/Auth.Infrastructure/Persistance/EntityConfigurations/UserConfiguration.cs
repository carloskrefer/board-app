using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistance.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property<Guid>(OptimisticConcurrencyHelper.VersionPropertyName).IsConcurrencyToken();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.HasIndex(e => e.Value).IsUnique();
            email.Property(e => e.Value).HasColumnName("email").IsRequired().HasMaxLength(255);
        });

        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.Value).HasColumnName("name").IsRequired().HasMaxLength(255);
        });

        builder.Ignore(u => u.NewSessions);

        builder.Navigation(u => u.Sessions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}