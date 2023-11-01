using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskTicketing.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions <AppDbContext> options):base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TicketUser>().HasKey(tu => new
            {

                tu.AppUserId,
                tu.TicketId

            });

            modelBuilder.Entity<TicketUser>().HasOne(t => t.Ticket).WithMany(tu => tu.TicketUsers).HasForeignKey(tu => tu.TicketId);
            modelBuilder.Entity<TicketUser>().HasOne(u => u.AppUser).WithMany(tu => tu.TicketUsers).HasForeignKey(tu => tu.AppUserId);

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<TicketFile> TicketFiles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<TicketUser> TicketUsers{ get; set; }
    }
}
