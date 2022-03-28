using System;
using System.Collections.Generic;
using IGames.Domain.Identity;

namespace IGames.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public User ApplicationUser { get; set; }
        public DateTime DateOfOrder { get; set; }
        public IEnumerable<VideoGameInOrder> VideoGamesInOrder { get; set; }
    }
}