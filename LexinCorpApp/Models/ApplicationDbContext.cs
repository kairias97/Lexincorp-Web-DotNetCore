﻿using Microsoft.EntityFrameworkCore;
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

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasAlternateKey(c => c.TributaryId)
                .HasName("UQ_Client_TributaryId");
        }
    }
}
