using IGames.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Services.Interface
{
    public interface IShoppingCartService
    {
        CartDTO getShoppingCartInfo(string userId);
        bool deleteVideoGameFromShoppingCart(string userId, Guid gameId);
        bool orderNow(string userId);
    }
}
