using KanbanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanBackend.Data
{
    // Ye class EF Core ko batati hai kaunse tables banane hain aur unke relationships kya hain
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Har DbSet ek table ko represent karta hai
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<BoardColumn> Columns { get; set; }
        public DbSet<TaskCard> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Email unique hona chahiye — duplicate accounts na bane
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships define karna (EF Core khud bhi guess kar leta hai, but explicit likhna best practice hai)
            modelBuilder.Entity<Board>()
                .HasOne(b => b.User)
                .WithMany(u => u.Boards)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<BoardColumn>()
                .HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId);

            modelBuilder.Entity<TaskCard>()
                .HasOne(t => t.Column)
                .WithMany(c => c.Cards)
                .HasForeignKey(t => t.ColumnId);
        }
    }
}
