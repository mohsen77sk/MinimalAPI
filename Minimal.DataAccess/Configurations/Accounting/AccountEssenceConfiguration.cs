using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountEssenceConfiguration : IEntityTypeConfiguration<AccountEssence>
{
    public void Configure(EntityTypeBuilder<AccountEssence> builder)
    {
        builder.ToTable("AccountEssences", Schema.Accounting);
        builder.HasIndex(accountEssence => accountEssence.Code).IsUnique();
    }
}
