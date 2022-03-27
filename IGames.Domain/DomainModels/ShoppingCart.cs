using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; set; }
        public User UserOwner { get; set; }
        public virtual ICollection<VideoGameInShoppingCart> VideoGamesInShoppingCart { get; set; }
    }
}
