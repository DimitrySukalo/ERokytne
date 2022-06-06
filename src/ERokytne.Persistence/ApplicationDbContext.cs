using ERokytne.Domain.Contracts;
using ERokytne.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERokytne.Persistence;

public class ApplicationDbContext : IdentityDbContext<Admin>
{
    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<TelegramUser> TelegramUsers { get; set; }
    
    public virtual DbSet<Photo> Photos { get; set; }
    
    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetCreatedFields();
        SetUpdatedFields();
            
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
        
    private void SetCreatedFields()
    {
        foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Added)
                     .Select(e => e.Entity))
        {
            if (entity is ITrackEntity trackEntity)
            {
                trackEntity.CreatedOn = DateTime.UtcNow;
            }
        }
    }

    private void SetUpdatedFields()
    {
        foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                     .Select(e => e.Entity))
        {
            if (entity is ITrackEntity trackEntity)
            {
                trackEntity.UpdatedOn = DateTime.UtcNow;
            }
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}