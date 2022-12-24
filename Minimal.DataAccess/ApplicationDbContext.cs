using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Minimal.Domain;
using Minimal.Domain.Identity;

namespace Minimal.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private IDbContextTransaction? _currentTransaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    #region BaseClass
    public DbSet<Person> People { set; get; } = default!;
    #endregion BaseClass

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction is null)
        {
            return;
        }

        await _currentTransaction.CommitAsync();

        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    public async Task RollbackTransaction()
    {
        if (_currentTransaction is null)
        {
            return;
        }

        await _currentTransaction.RollbackAsync();

        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users", Schema.Auth); // Your custom IdentityUser class
        builder.Entity<IdentityRole>().ToTable("Roles", Schema.Auth);
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", Schema.Auth);
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", Schema.Auth);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", Schema.Auth);
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", Schema.Auth);
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", Schema.Auth);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
