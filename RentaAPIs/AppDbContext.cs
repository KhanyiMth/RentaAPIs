using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.Models;

namespace RentaAPIs
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options)
        {

        }
        
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PropertyApplication> PropertyApplications { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<PropertyEligibilityPrediction> PropertyEligibilityPredictions { get; set; }
        public DbSet<ViewingBooking> ViewingBookings { get; set; }
        public DbSet<ViewingAvailability> ViewingAvailabilities { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<TenantApplication> TenantApplications { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Landlord>().ToTable("landlords");

            modelBuilder.Entity<Lease>().ToTable("leases");

            modelBuilder.Entity<MaintenanceRequest>().ToTable("maintenanceRequests");

            modelBuilder.Entity<Payment>().ToTable("payments");

            modelBuilder.Entity<Property>().ToTable("properties");

            modelBuilder.Entity<Tenant>().ToTable("tenants");

            modelBuilder.Entity<Unit>().ToTable("units");

            modelBuilder.Entity<Document>().ToTable("documents");

            modelBuilder.Entity<Notification>().ToTable("notifications");

            modelBuilder.Entity<PropertyApplication>().ToTable("propertyApplications");

            modelBuilder.Entity<Amenity>().ToTable("amenities");

            modelBuilder.Entity<PropertyEligibilityPrediction>().ToTable("propertyEligibilityPrediction");

            modelBuilder.Entity<Advertisement>().ToTable("advertisements");

            modelBuilder.Entity<TenantApplication>().ToTable("tenantapplications");

            foreach(var foreignKey in modelBuilder.Model
                                    .GetEntityTypes()
                                    .SelectMany (e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }

            modelBuilder.Entity<PropertyEligibilityPrediction>()
                .HasOne(p => p.Tenant)
                .WithMany()
                .HasForeignKey(p => p.TenantID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PropertyEligibilityPrediction>()
                .HasOne(p => p.Property)
                .WithMany()
                .HasForeignKey(p => p.PropertyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.Amenities)
                .WithMany(a => a.Properties)
                .UsingEntity(j => j.ToTable("PropertyAmenities"));

            modelBuilder.Entity<ViewingAvailability>()
                .HasOne(v => v.Property)
                .WithMany(p => p.ViewingAvailabilities)
                .HasForeignKey(v => v.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ViewingBooking>()
                .HasOne(b => b.ViewingAvailability)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.ViewingAvailabilityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViewingBooking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.ViewingBookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Advertisement>()
                .HasOne(a => a.Property)
                .WithMany()
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Advertisement>()
                .HasOne(a => a.Unit)
                .WithMany()
                .HasForeignKey(a => a.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TenantApplication>()
                .HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TenantApplication>()
                .HasOne(a => a.Advertisement)
                .WithMany()
                .HasForeignKey(a => a.AdvertisementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TenantApplication>()
                .HasOne(a => a.Property)
                .WithMany()
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TenantApplication>()
                .HasOne(a => a.Unit)
                .WithMany()
                .HasForeignKey(a => a.UnitId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Document>()
                .HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Tenant)
                .WithMany()
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Lease)
                .WithMany()
                .HasForeignKey(d => d.LeaseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.MaintenanceRequest)
                .WithMany()
                .HasForeignKey(d => d.MaintenanceRequestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
                .Property(d => d.FileData)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.AmountPaid)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Lease)
                .WithMany(l => l.Payments)
                .HasForeignKey(p => p.LeaseID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => new
                {
                    p.LeaseID,
                    p.RentalPeriod
                })
                .IsUnique()
                .HasFilter("[RentalPeriod] IS NOT NULL");
        }

    }
}
