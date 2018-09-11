using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LexincorpApp.CronJob.Models
{
    public partial class LexincorpAdminContext : DbContext
    {
        public LexincorpAdminContext()
        {
        }

        public LexincorpAdminContext(DbContextOptions<LexincorpAdminContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BillableRetainers> BillableRetainers { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Retainers> Retainers { get; set; }
        public virtual DbSet<RetainerSubscriptions> RetainerSubscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LexincorpAdmin;Trusted_Connection=True; MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillableRetainers>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.HasIndex(e => e.CreatorId);

                entity.HasIndex(e => e.RetainerId);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.BillableRetainers)
                    .HasForeignKey(d => d.ClientId);

                entity.HasOne(d => d.Retainer)
                    .WithMany(p => p.BillableRetainers)
                    .HasForeignKey(d => d.RetainerId);
            });

            modelBuilder.Entity<Clients>(entity =>
            {
                entity.HasIndex(e => e.BillingModeId);

                entity.HasIndex(e => e.ClientTypeId);

                entity.HasIndex(e => e.DocumentDeliveryMethodId);

                entity.HasIndex(e => e.TributaryId)
                    .HasName("UQ_Client_TributaryId")
                    .IsUnique();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Contact).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.TributaryId).IsRequired();
            });

            modelBuilder.Entity<Retainers>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SpanishName).IsRequired();
            });

            modelBuilder.Entity<RetainerSubscriptions>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.HasIndex(e => e.CreatorId);

                entity.HasIndex(e => e.RetainerId);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.RetainerSubscriptions)
                    .HasForeignKey(d => d.ClientId);

                entity.HasOne(d => d.Retainer)
                    .WithMany(p => p.RetainerSubscriptions)
                    .HasForeignKey(d => d.RetainerId);
            });
        }
    }
}
