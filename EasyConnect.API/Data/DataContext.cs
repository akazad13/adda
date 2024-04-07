using EasyConnect.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyConnect.API.Data
{
    public class DataContext(DbContextOptions<DataContext> options)
                : IdentityDbContext<
            User,
            Role,
            int,
            IdentityUserClaim<int>,
            UserRole,
            IdentityUserLogin<int>,
            IdentityRoleClaim<int>,
            IdentityUserToken<int>
        >(options)
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
                userRole
                    .HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Bookmark>().HasKey(k => new { k.BookmarkerId, k.BookmarkedId });

            builder
                .Entity<Bookmark>()
                .HasOne(u => u.Bookmarked)
                .WithMany(u => u.Bookmarkers)
                .HasForeignKey(u => u.BookmarkedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Bookmark>()
                .HasOne(u => u.Bookmarker)
                .WithMany(u => u.Bookmarkeds)
                .HasForeignKey(u => u.BookmarkerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
        }
    }
}
