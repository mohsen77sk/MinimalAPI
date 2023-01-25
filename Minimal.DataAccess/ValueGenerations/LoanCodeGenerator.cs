using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Minimal.Domain;

namespace Minimal.DataAccess.ValueGenerations;

public class LoanCodeGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => NextCode(entry);

    private string NextCode(EntityEntry entry)
    {
        var _lastLoanCode = "";
        var _typeCode = "0000";
        var _code = "0000";

        var _context = (ApplicationDbContext)entry.Context;
        var _lon = (Loan)entry.Entity;

        if (_lon.LoanType is not null)
        {
            _typeCode = _context.LoanTypes
                .AsNoTracking()
                .Where(x => x.Id == _lon.LoanTypeId)
                .Select(x => x.Code)
                .SingleOrDefault() ?? "0000";

            _lastLoanCode = _context.Loans
                .AsNoTracking()
                .Where(x => x.LoanTypeId == _lon.LoanTypeId)
                .OrderByDescending(x => x.Code)
                .Select(x => x.Code)
                .FirstOrDefault() ?? "00000000";
        }

        _lastLoanCode = (_lastLoanCode).Substring(4);
        _code = (int.Parse(_lastLoanCode) + 1).ToString().PadLeft(4, '0');

        return _typeCode + _code;
    }
}