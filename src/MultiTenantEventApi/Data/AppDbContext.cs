using Microsoft.EntityFrameworkCore;
using MultiTenantEventApi.Models;
using MultiTenantEventApi.Tenancy;

namespace MultiTenantEventApi.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IOrganisationContext organisationContext) : DbContext(options)
{
    private Guid CurrentOrganisationId => organisationContext.OrganisationId;

    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<CommunityEvent> Events => Set<CommunityEvent>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organisation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<CommunityEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.HasOne(x => x.Organisation)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.OrganisationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(x => x.OrganisationId == CurrentOrganisationId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
            entity.HasOne(x => x.Event)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.OrganisationId, x.EventId, x.NormalizedEmail }).IsUnique();
            entity.HasQueryFilter(x => x.OrganisationId == CurrentOrganisationId);
        });
    }
}
