using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Minimal.Domain;

namespace Minimal.DataAccess.ValueGenerations;

public class AccountCodeGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => NextCode(entry);

    private string NextCode(EntityEntry entry)
    {
        var _lastAccountCode = "";
        var _typeCode = "0000";
        var _code = "0000";

        var _context = (ApplicationDbContext)entry.Context;
        var _acc = (Account)entry.Entity;

        if (_acc.AccountType is not null)
        {
            _typeCode = _context.AccountTypes
                .AsNoTracking()
                .Where(x => x.Id == _acc.AccountTypeId)
                .Select(x => x.Code)
                .SingleOrDefault() ?? "0000";

            _lastAccountCode = _context.Accounts
                .AsNoTracking()
                .Where(x => x.AccountTypeId == _acc.AccountTypeId)
                .OrderByDescending(x => x.Code)
                .Select(x => x.Code)
                .FirstOrDefault() ?? "00000000";
        }

        _lastAccountCode = (_lastAccountCode).Substring(4);
        _code = (int.Parse(_lastAccountCode) + 1).ToString().PadLeft(4, '0');

        return _typeCode + _code;
    }
}