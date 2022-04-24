using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DomainModels
{
    public class VideoGame : BaseEntity
    {
        public string GameTitle { get; set; }
        public string Image { get; set; }
        public GenreEnum Genre { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public virtual ICollection<VideoGameInShoppingCart> VideoGamesInShoppingCart { get; set; }
        public IEnumerable<VideoGameInOrder> VideoGamesInOrder { get; set; }
    }
}
