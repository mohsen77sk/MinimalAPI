using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.DataAccess.ValueGenerations;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans", Schema.App);
        builder.HasIndex(loan => loan.Code).IsUnique();
        builder.Property(loan => loan.Code).ValueGeneratedOnAdd().HasValueGenerator<LoanCodeGenerator>();
        builder.Property(loan => loan.Amount).HasColumnType("Money");
        builder.Property(loan => loan.InstallmentAmount).HasColumnType("Money");
    }
}
