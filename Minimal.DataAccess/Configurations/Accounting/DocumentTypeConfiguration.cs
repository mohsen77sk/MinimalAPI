using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ToTable("DocumentTypes", Schema.Accounting);
        builder.HasIndex(documentType => documentType.Code).IsUnique();
    }
}
