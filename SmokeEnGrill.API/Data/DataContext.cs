using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
    UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<InventOp> InventOps { get; set; }
        public DbSet<InventOpType> InventOpTypes { get; set; }

        public DbSet<StockMvt> StockMvts { get; set; }
        public DbSet<StockMvtInventOp> StockMvtInventOps { get; set; }
        public DbSet<Store> Stores { get; set; }

        public DbSet<StoreType> StoreTypes { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }
        public DbSet<OrderLineProduct> OrderLineProducts { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<MenuProduct> menuProducts { get; set; }















        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }
    }
}