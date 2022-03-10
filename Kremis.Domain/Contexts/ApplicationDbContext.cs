using System;
using Kremis.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kremis.Domain.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AddUniqueKeyConstraints(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<ParcelDocument> ParcelDocuments { get; set; }
        public DbSet<LandTitle> LandTitles { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<LandTitleDocument> LandTitleDocuments { get; set; }
        public DbSet<IdentityDocumentType> IdentityDocumentTypes { get; set; }
        public DbSet<CustomerDocument> CustomerDocuments { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<CommissionPayment> CommissionPayments { get; set; }
        public DbSet<Locality> Localities { get; set; }
        public DbSet<PaymentMode> PaymentModes { get; set; }

        private static void AddUniqueKeyConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LandTitle>().HasIndex(u => u.Number).IsUnique();
            modelBuilder.Entity<Parcel>().HasIndex(u => new { u.Number, u.LandTitleId }).IsUnique();
            modelBuilder.Entity<DocumentType>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<LandTitleDocument>().HasIndex(u => new { u.LandTitleId, u.DocumentTypeId, u.DocumentUrl }).IsUnique();
            modelBuilder.Entity<IdentityDocumentType>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<InvoiceDetail>().HasIndex(u => u.ParcelId).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(u => new { u.IdentityDocumentTypeId, u.IdentityDocumentNumber }).IsUnique();
            modelBuilder.Entity<CustomerDocument>().HasIndex(u => new { u.CustomerId, u.DocumentTypeId, u.DocumentUrl }).IsUnique();
            modelBuilder.Entity<Locality>().HasIndex(u => new { u.Name, u.CityId }).IsUnique();
            modelBuilder.Entity<InvoicePayment>().HasIndex(u => new { u.PaymentModeId, u.TransactionNumber }).IsUnique();
            modelBuilder.Entity<CommissionPayment>().HasIndex(u => new { u.PaymentModeId, u.TransactionNumber }).IsUnique();
        }
    }
}
