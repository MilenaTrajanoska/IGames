using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DomainModels
{
    public class VideoGameInShoppingCart : BaseEntity
    {
        public Guid VideoGameId { get; set; }
        public VideoGame VideoGame { get; set; }
        public Guid CartId { get; set; }
        public ShoppingCart UserCart { get; set; }
        public int Quantity { get; set; }
    }
}
