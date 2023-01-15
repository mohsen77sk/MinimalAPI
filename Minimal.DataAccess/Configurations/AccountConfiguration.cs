using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", Schema.App);
        builder.HasIndex(account => account.Code).IsUnique();
        builder.Property(account => account.Code).HasDefaultValueSql();
        builder.Property(account => account.IsActive).HasDefaultValue(true);
    }
}
