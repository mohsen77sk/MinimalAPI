using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountLedgerConfiguration : IEntityTypeConfiguration<AccountLedger>
{
    public void Configure(EntityTypeBuilder<AccountLedger> builder)
    {
        builder.ToTable("AccountLedgers", Schema.Accounting);
        builder.HasIndex(accountLedger => accountLedger.Code).IsUnique();
    }
}
