using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Minimal.DataAccess.ValueGenerations;

public class DocumentCodeGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => NextCode(entry);

    private string NextCode(EntityEntry entry)
    {
        var _context = (ApplicationDbContext)entry.Context;

        var lastDocumentCode =
            _context.Documents.Local.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault() ??
            (_context.Documents.AsNoTracking().OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault() ?? "0");

        return (int.Parse(lastDocumentCode ?? "0") + 1).ToString().PadLeft(10, '0');
    }
}