using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountGroupConfiguration : IEntityTypeConfiguration<AccountGroup>
{
    public void Configure(EntityTypeBuilder<AccountGroup> builder)
    {
        builder.ToTable("AccountGroups", Schema.Accounting);
        builder.HasIndex(accountGroup => accountGroup.Code).IsUnique();
    }
}
