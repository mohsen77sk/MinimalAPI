using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Minimal.DataAccess.ValueGenerations;

public class PersonCodeGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => NextCode(entry);

    private string NextCode(EntityEntry entry)
    {
        var _context = (ApplicationDbContext)entry.Context;

        var lastAccountCode = int.Parse(
            _context.People.AsNoTracking().OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault() ?? "0"
        );

        lastAccountCode++;

        return lastAccountCode.ToString().PadLeft(4, '0');
    }
}