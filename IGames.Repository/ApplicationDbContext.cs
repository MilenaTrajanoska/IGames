using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using IGames.Domain.DomainModels;
using IGames.Domain.Identity;

namespace IGames.Repository
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<VideoGames> VideoGames { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<VideoGamesInShoppingCart> VideoGamesInShoppingCarts { get; set; }
        public virtual DbSet<VideoGamesInOrder> VideoGamesInOrders { get; set; }
        public virtual DbSet<OrderEmailMessage> EmailMessages { get; set; }
 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Role.STANDARD_USER, NormalizedName = Role.STANDARD_USER, Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Role.ADMIN, NormalizedName = Role.ADMIN, Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });

            builder.Entity<VideoGames>()
                .Property(vg => vg.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Cart>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Order>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<VideoGamesInShoppingCart>()
               .HasOne(visc => visc.VideoGames)
               .WithMany(vg => vg.VideoGamesInShoppingCart)
               .HasForeignKey(visc => visc.TicketId);

            builder.Entity<VideoGamesInShoppingCart>()
                .HasOne(vgisc => vgisc.UserCart)
                .WithMany(uc => uc.VideoGamesInShoppingCart)
                .HasForeignKey(vgisc => vgisc.CartId);

            builder.Entity<VideoGamesInOrder>()
                .HasOne(vgio => vgio.ticket)
                .WithMany(v => v.VideoGamesInOrder)
                .HasForeignKey(vgio => vgio.TicketId);

            builder.Entity<VideoGamesInOrder>()
                .HasOne(vgio => vgio.order)
                .WithMany(o => o.VideoGamesInOrder)
                .HasForeignKey(vgio => vgio.OrderId);

            builder.Entity<Cart>()
                .HasOne<User>(z => z.UserOwner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<Cart>(z => z.UserId);

        }

    }
}
