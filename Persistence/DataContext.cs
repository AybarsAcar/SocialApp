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

    // the Join Table
    public DbSet<UserFollowing> UserFollowings { get; set; }
    
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
      
      // configure the relationship in the join table
      // for many to many follower / followee relationship
      builder.Entity<UserFollowing>(b =>
      {
        b.HasKey(key => new {key.ObserverId, key.TargetId});

        b.HasOne(o => o.Observer)
          .WithMany(f => f.Followings)
          .HasForeignKey(o => o.ObserverId)
          .OnDelete(DeleteBehavior.Cascade);
        
        b.HasOne(o => o.Target)
          .WithMany(f => f.Followers)
          .HasForeignKey(o => o.TargetId)
          .OnDelete(DeleteBehavior.Cascade);
      });
    }
  }
}