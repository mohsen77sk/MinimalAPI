using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class LoanPaymentConfiguration : IEntityTypeConfiguration<LoanPayment>
{
    public void Configure(EntityTypeBuilder<LoanPayment> builder)
    {
        builder.ToTable("LoanPayments", Schema.App);
        builder.Property(loanPayment => loanPayment.Amount).HasColumnType("Money");
        builder.Property(loanPayment => loanPayment.Note).HasDefaultValue(string.Empty);
    }
}
