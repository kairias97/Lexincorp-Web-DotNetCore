using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<BillingMode> BillingModes { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<DocumentDeliveryMethod> DocumentDeliveryMethods { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attorney> Attorneys { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<VacationsMovement> VacationsMovements { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<VacationsRequest> VacationsRequests { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Retainer> Retainers { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<RetainerSubscription> RetainerSubscriptions { get; set; }
        public DbSet<BillableRetainer> BillableRetainers { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityExpense> ActivityExpenses { get; set; }
        public DbSet<BillHeader> BillHeaders { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<ClosureNotification> ClosureNotifications { get; set; }
        public DbSet<NotificationAnswer> NotificationAnswers { get; set; }
        public DbSet<VacationsRequestAnswer> VacationsRequestAnswers { get; set; }
        public DbSet<VacationsMonthlyCredit> VacationsMonthlyCredits { get; set; }
        public DbSet<ClientDeposit> ClientDeposits { get; set; }
        public virtual DbSet<BillSummary> BillSummaries { get; set; }
        public virtual DbSet<BillSummary> BillExpenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasAlternateKey(c => c.TributaryId)
                .HasName("UQ_Client_TributaryId");
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username)
                .HasName("UQ_User_Username");
            modelBuilder.Entity<User>()
                .Property(u => u.Active)
                .HasDefaultValue(true);
            modelBuilder.Entity<Attorney>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<Attorney>()
                .HasIndex(a => a.NotaryCode)
                .IsUnique();
            modelBuilder.Entity<Attorney>()
                .Property(a => a.VacationCount)
                .HasDefaultValue(0);
            modelBuilder.Entity<VacationsMovement>()
                .Property(v => v.Created)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<VacationsRequest>()
                .Property(v => v.Created)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Category>()
                .Property(c => c.Active)
                .HasDefaultValue(true);
            modelBuilder.Entity<Service>()
                .Property(c => c.Active)
                .HasDefaultValue(true);
            modelBuilder.Entity<Retainer>()
                .Property(c => c.Active)
                .HasDefaultValue(true);
            modelBuilder.Entity<Package>()
                .Property(p => p.IsFinished)
                .HasDefaultValue(false);
            modelBuilder.Entity<Client>()
                .Property(c => c.Active)
                .HasDefaultValue(true);
            modelBuilder.Entity<BillableRetainer>()
                .Property(b => b.ConsumedHours)
                .HasDefaultValue(0);
            modelBuilder.Entity<Package>()
                .Property(p => p.IsBilled)
                .HasDefaultValue(false);
            modelBuilder.Entity<Package>()
                .HasOne<User>(p => p.CreatorUser)
                .WithMany(u => u.Packages)
                .HasForeignKey(p => p.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Attorney>()
                .Property(a => a.CanApproveVacations)
                .HasDefaultValue(true);
            modelBuilder.Entity<VacationsRequest>()
                .Property(vr => vr.IsApplied)
                .HasDefaultValue(false);

            modelBuilder.Entity<VacationsRequestAnswer>()
                .HasOne(vra => vra.Request)
                .WithMany(request => request.VacationsRequestAnswers)
                .HasForeignKey(vra => vra.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Package>()
                .Property(p => p.AgreedExpensesAmount)
                .HasDefaultValue(0);
            modelBuilder.Entity<Activity>()
                .Property(a => a.IsBillable)
                .HasDefaultValue(false);
            modelBuilder.Entity<ClientDeposit>()
                .HasOne(cd => cd.Package)
                .WithMany(package => package.ClientDeposits)
                .HasForeignKey(cd => cd.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
