using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountDetailConfiguration : IEntityTypeConfiguration<AccountDetail>
{
    public void Configure(EntityTypeBuilder<AccountDetail> builder)
    {
        builder.ToTable("AccountDetails", Schema.Accounting);
        builder.HasIndex(accountDetail => accountDetail.Code).IsUnique();
        builder.Property(accountDetail => accountDetail.Level).HasDefaultValue(0);
    }
}
