using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanInstallmentConfiguration : IEntityTypeConfiguration<LoanInstallment>
{
    public void Configure(EntityTypeBuilder<LoanInstallment> builder)
    {
        builder.ToTable("LoanInstallments", Schema.App);
        builder.Property(loanInstallment => loanInstallment.Amount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.PrincipalAmount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.InterestAmount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.PaidAmount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.Status).HasDefaultValue(InstallmentStatus.Pending);
    }
}
