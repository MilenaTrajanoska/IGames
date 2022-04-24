using IGames.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DTO
{
    public class CartDTO
    {
        public List<VideoGameInShoppingCart> Games { get; set; }
        public double TotalPrice { get; set; }

    }
}
