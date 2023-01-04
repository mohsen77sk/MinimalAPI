using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.ToTable("AccountTypes", Schema.App);
        builder.HasIndex(accountType => accountType.Code).IsUnique();
        builder.Property(accountType => accountType.IsActive).HasDefaultValue(true);
    }
}
