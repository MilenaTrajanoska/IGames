using System;
using System.Collections.Generic;
using TicketShop.Domain.Identity;

namespace TicketShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public User ApplicationUser { get; set; }
        public DateTime DateOfOrder { get; set; }
        public IEnumerable<VideoGameInOrder> VideoGamesInOrder { get; set; }
    }
}