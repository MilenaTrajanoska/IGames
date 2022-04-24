using IGames.Domain.DomainModels;
using IGames.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IGames.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApplicationDbContext context;
        private DbSet<Order> orders;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            orders = context.Set<Order>();
        }

        public List<Order> getAllOrders()
        {
            return orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.VideoGamesInOrder)
                .Include("VideoGamesInOrder.Game")
                .ToListAsync().Result;
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return orders
               .Include(o => o.ApplicationUser)
               .Include(o => o.VideoGamesInOrder)
               .Include("VideoGamesInOrder.Game")
               .SingleOrDefaultAsync(o => o.Id == model.Id).Result;
        }
    }
}
