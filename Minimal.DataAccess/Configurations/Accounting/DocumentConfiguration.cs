using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents", Schema.Accounting);
        builder.HasIndex(document => document.Code).IsUnique();
        builder.Property(document => document.IsActive).HasDefaultValue(true);
    }
}
