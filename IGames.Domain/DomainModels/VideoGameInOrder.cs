using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DomainModels
{
    public class VideoGameInOrder : BaseEntity
    {
        public Guid VideoGameId { get; set; }
        public VideoGame Game { get; set; }
        public Guid OrderId { get; set; }
        public Order order { get; set; }
        public int Quantity { get; set; }
    }
}
