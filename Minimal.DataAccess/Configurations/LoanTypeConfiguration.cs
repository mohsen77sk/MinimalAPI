using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
{
    public void Configure(EntityTypeBuilder<LoanType> builder)
    {
        builder.ToTable("LoanTypes", Schema.App);
        builder.HasIndex(loanType => loanType.Code).IsUnique();
    }
}
