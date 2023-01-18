using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.DataAccess.ValueGenerations;
using Minimal.Domain;

namespace Minimal.DataAccess.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People", Schema.App);
        builder.HasIndex(person => person.Code).IsUnique();
        builder.Property(person => person.Code).ValueGeneratedOnAdd().HasValueGenerator<PersonCodeGenerator>();
        builder.Ignore(person => person.FullName);
    }
}
