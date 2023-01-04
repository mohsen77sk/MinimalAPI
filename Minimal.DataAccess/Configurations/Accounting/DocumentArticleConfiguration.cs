using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class DocumentArticleConfiguration : IEntityTypeConfiguration<DocumentArticle>
{
    public void Configure(EntityTypeBuilder<DocumentArticle> builder)
    {
        builder.ToTable("DocumentArticles", Schema.Accounting);
        builder.Property(documentArticles => documentArticles.Debit).HasColumnType("Money");
        builder.Property(documentArticles => documentArticles.Credit).HasColumnType("Money");
    }
}
