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

            public virtual DbSet<VideoGame> VideoGames { get; set; }
            public virtual DbSet<ShoppingCart> Carts { get; set; }
            public virtual DbSet<Order> Orders { get; set; }
            public virtual DbSet<VideoGameInShoppingCart> VideoGameInShoppingCarts { get; set; }
            public virtual DbSet<VideoGameInOrder> VideoGameInOrders { get; set; }
            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Role.STANDARD_USER, NormalizedName = Role.STANDARD_USER, Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
                builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Role.ADMIN, NormalizedName = Role.ADMIN, Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });

                builder.Entity<VideoGame>()
                    .Property(g => g.Id)
                    .ValueGeneratedOnAdd();

                builder.Entity<ShoppingCart>()
                    .Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                builder.Entity<Order>()
                    .Property(o => o.Id)
                    .ValueGeneratedOnAdd();

                builder.Entity<VideoGameInShoppingCart>()
                   .HasOne(visc => visc.VideoGame)
                   .WithMany(v => v.VideoGamesInShoppingCart)
                   .HasForeignKey(visc => visc.VideoGameId);

                builder.Entity<VideoGameInShoppingCart>()
                    .HasOne(v => v.UserCart)
                    .WithMany(v => v.VideoGamesInShoppingCart)
                    .HasForeignKey(v =>  v.CartId);

                builder.Entity<VideoGameInOrder>()
                    .HasOne(vio => vio.Game)
                    .WithMany(v => v.VideoGamesInOrder)
                    .HasForeignKey(vio => vio.VideoGameId);

                builder.Entity<VideoGameInOrder>()
                    .HasOne(vio => vio.order)
                    .WithMany(o => o.VideoGamesInOrder)
                    .HasForeignKey(vio => vio.OrderId);

                builder.Entity<ShoppingCart>()
                    .HasOne<User>(z => z.UserOwner)
                    .WithOne(z => z.UserCart)
                    .HasForeignKey<ShoppingCart>(z => z.UserId);

            }

        }
    }
