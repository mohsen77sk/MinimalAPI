using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class AccountCategoryConfiguration : IEntityTypeConfiguration<AccountCategory>
{
    public void Configure(EntityTypeBuilder<AccountCategory> builder)
    {
        builder.ToTable("AccountCategories", Schema.Accounting);
        builder.HasIndex(accountCategory => accountCategory.Code).IsUnique();
    }
}
