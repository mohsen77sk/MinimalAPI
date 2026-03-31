using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanInstallmentConfiguration : IEntityTypeConfiguration<LoanInstallment>
{
    public void Configure(EntityTypeBuilder<LoanInstallment> builder)
    {
        builder.ToTable("LoanInstallments", Schema.App);
        builder.Property(loanInstallment => loanInstallment.PrincipalAmount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.InterestAmount).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.PaidPrincipal).HasColumnType("Money");
        builder.Property(loanInstallment => loanInstallment.PaidInterest).HasColumnType("Money");
        builder.Ignore(loanInstallment => loanInstallment.Amount);
        builder.Ignore(loanInstallment => loanInstallment.PaidAmount);
        builder.Property(loanInstallment => loanInstallment.Status).HasDefaultValue(InstallmentStatus.Pending);
    }
}
