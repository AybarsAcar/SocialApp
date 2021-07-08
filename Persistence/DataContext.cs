using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  public class DataContext : IdentityDbContext<AppUser>
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    // Database table
    public DbSet<Activity> Activities { get; set; }

    public DbSet<ActivityAttendee> ActivityAttendees { get; set; }

    public DbSet<Photo> Photos { get; set; }

    public DbSet<Comment> Comments { get; set; }
    
    /// <summary>
    /// Set our many to many relationship
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder
        .Entity<ActivityAttendee>(x =>
          x.HasKey(aa => new {aa.AppUserId, aa.ActivityId}));

      builder.Entity<ActivityAttendee>()
        .HasOne(u => u.AppUser)
        .WithMany(a => a.Activities)
        .HasForeignKey(aa => aa.AppUserId);
      
      builder.Entity<ActivityAttendee>()
        .HasOne(u => u.Activity)
        .WithMany(a => a.Attendees)
        .HasForeignKey(aa => aa.ActivityId);

      // if the Activity is deleted
      // the comments are also deleted from the database
      builder.Entity<Comment>()
        .HasOne(a => a.Activity)
        .WithMany(c => c.Comments)
        .OnDelete(DeleteBehavior.Cascade);
      // Author Delete is Restrict
      // because we want to maintain the comments of the deleted users
    }
  }
}