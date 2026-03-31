using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanPaymentAllocationConfiguration : IEntityTypeConfiguration<LoanPaymentAllocation>
{
    public void Configure(EntityTypeBuilder<LoanPaymentAllocation> builder)
    {
        builder.ToTable("LoanPaymentAllocations", Schema.App);
        builder.Property(loanPaymentAllocation => loanPaymentAllocation.PrincipalAmount).HasColumnType("Money");
        builder.Property(loanPaymentAllocation => loanPaymentAllocation.InterestAmount).HasColumnType("Money");
        builder.Ignore(loanPaymentAllocation => loanPaymentAllocation.TotalAmount);
        builder.HasOne(loanPaymentAllocation => loanPaymentAllocation.Installment)
            .WithMany(loanInstallment => loanInstallment.PaymentAllocations)
            .HasForeignKey(loanPaymentAllocation => loanPaymentAllocation.InstallmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
