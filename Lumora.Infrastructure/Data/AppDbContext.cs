using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Entities.Reviews;
using Lumora.Domain.Entities.Studio;
using Lumora.Domain.Entities.Tag;
using Lumora.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ConsumerProfile> ConsumerProfiles { get; set; }
    public DbSet<StudioProfile> StudioProfiles { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<PortfolioImage> PortfolioImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //call configuration extension methods
        modelBuilder.ConfigureUser();
        modelBuilder.ConfigureConsumerProfile();
        modelBuilder.ConfigureStudioProfile();
        modelBuilder.ConfigureEmployee();
        modelBuilder.ConfigureTag();
        modelBuilder.ConfigureEvent();
        modelBuilder.ConfigureInquiry();
        modelBuilder.ConfigurePayment();
        modelBuilder.ConfigureGallery();
        modelBuilder.ConfigureReview();
        modelBuilder.ConfigurePortoflioImage();

    }

}
