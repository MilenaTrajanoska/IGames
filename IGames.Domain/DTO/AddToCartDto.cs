using IGames.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DTO
{
    public class AddToCartDTO
    {
        public VideoGame SelectedVideoGame { get; set; }
        public Guid VideoGameId { get; set; }
        public int Quantity { get; set; }
    }
}
