using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Minimal.Domain;

namespace Minimal.DataAccess.ValueGenerations;

public class AccountDetailCodeGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => NextCode(entry);

    private string NextCode(EntityEntry entry)
    {
        var code = "00000000";
        var _context = (ApplicationDbContext)entry.Context;
        var _accDetail = (AccountDetail)entry.Entity;

        if (_accDetail.Account is not null)
        {
            code = _accDetail.Account.Code;
        }

        if (_accDetail.Loan is not null)
        {
            code = _accDetail.Loan.Code;
        }

        return code;
    }
}