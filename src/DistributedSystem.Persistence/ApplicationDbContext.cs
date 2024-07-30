using DistributedSystem.Domain.Entities;
using DistributedSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystem.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlServer("Data Source=(localdb)\\v11.0;Initial Catalog=DistributedSystem;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=True;Trust Server Certificate=False;Command Timeout=0");

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Domain.Entities.Identity.Action> Actions { get; set; }
    public DbSet<Function> Functions { get; set; }
    public DbSet<ActionInFunction> ActionsInFunctions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Product> Products { get; set; }
}
