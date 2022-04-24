using IGames.Domain.DomainModels;
using IGames.Domain.DTO;
using System;
using System.Collections.Generic;

namespace IGames.Services.Interface
{
    public interface IVideoGameService
    {
        List<VideoGame> GetAllVideoGames();
        VideoGame GetDetailsForVideoGame(Guid? id);
        void CreateNewVideoGame(VideoGame game);
        void UpdeteExistingVideoGame(VideoGame game);
        AddToCartDTO GetShoppingCartInfo(Guid? id);
        void DeleteVideoGame(Guid id);
        bool AddVideoGameToShoppingCart(AddToCartDTO item, string userID);
        List<VideoGame> FilterVideoGamesByGenre(GenreEnum genre);
        public byte[] ExportAllVideoGames();
        public byte[] ExportVideoGamesByGenre(GenreEnum genre);
    }
}
