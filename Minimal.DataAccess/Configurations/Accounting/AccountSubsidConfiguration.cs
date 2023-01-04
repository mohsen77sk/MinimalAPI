using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountSubsidConfiguration : IEntityTypeConfiguration<AccountSubsid>
{
    public void Configure(EntityTypeBuilder<AccountSubsid> builder)
    {
        builder.ToTable("AccountSubsids", Schema.Accounting);
        builder.HasIndex(accountSubsid => accountSubsid.Code).IsUnique();
    }
}
