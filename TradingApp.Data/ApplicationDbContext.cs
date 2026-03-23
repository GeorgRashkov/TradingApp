using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;

namespace TradingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Balance> Balances { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ReportedProduct> ReportedProducts { get; set; } = null!;


        public virtual DbSet<SellOrder> SellOrders { get; set; } = null!;
        public virtual DbSet<OrderRequest> OrderRequests { get; set; } = null!;

        public virtual DbSet<SellOrderSuggestion> SellOrderSuggestions { get; set; } = null!;
        public virtual DbSet<CompletedOrder> CompletedOrders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureSellOrder(builder);
            ConfigureReportedProduct(builder);
            ConfigureSellOrderSuggestion(builder);
            ConfigureCompletedOrder(builder);

        }

        public void ConfigureSellOrder(ModelBuilder builder)
        {
            builder.Entity<SellOrder>(e =>
            {
                e.HasOne(so => so.Product)
                .WithMany(p => p.SellOrders)
                .HasForeignKey(so => so.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public void ConfigureReportedProduct(ModelBuilder builder)
        {
            builder.Entity<ReportedProduct>(e =>
            {
                e.HasOne(rp => rp.Product)
                .WithMany(p => p.ReportedProducts)
                .HasForeignKey(rp => rp.ReportedProductId)
                .OnDelete(DeleteBehavior.Restrict);
            }
            );
        }

        public void ConfigureSellOrderSuggestion(ModelBuilder builder)
        {
            builder.Entity<SellOrderSuggestion>(e =>
            {
                e.HasOne(sos => sos.Product)
                .WithMany(p => p.SellOrderSuggestions)
                .HasForeignKey(sos => sos.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SellOrderSuggestion>(e =>
            {
                e.HasOne(sos => sos.OrderRequest)
                .WithMany(p => p.SellOrderSuggestions)
                .HasForeignKey(sos => sos.OrderRequestId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureCompletedOrder(ModelBuilder builder)
        {
            builder.Entity<CompletedOrder>(e =>
            {
                e.HasOne(co => co.Product)
                .WithMany(p => p.CompletedOrders)
                .HasForeignKey(co => co.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(co => co.Buyer).
                WithMany(b => b.CompletedPurchaseOrders)
                .HasForeignKey(co => co.BuyerId)
                .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(co => co.Seller)
                .WithMany(s => s.CompletedSellOrders)
                .HasForeignKey(co => co.SellerId)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
