using ClosedXML.Excel;
using IGames.Domain.DomainModels;
using IGames.Repository.Interface;
using IGames.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IGames.Services.Implementation
{
    public class VideoGameService : IVideoGameService
    {
        private readonly IRepository<VideoGame> _videoGameRepository;
        private readonly IRepository<VideoGameInShoppingCart> _videoGameInShoppingCartRepository;
        private readonly IUserRepository _userRepository;

        public VideoGameService(IRepository<VideoGame> videoGameRepository, IRepository<VideoGameInShoppingCart> videoGameInShoppingCartRepository, IUserRepository userRepository)
        {
            _videoGameRepository = videoGameRepository;
            _userRepository = userRepository;
            _videoGameInShoppingCartRepository = videoGameInShoppingCartRepository;
        }

        public bool AddVideoGameToShoppingCart(AddToCartDTO item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCard = user.UserCart;

            if (item.VideoGameId != null && userShoppingCard != null)
            {
                var game = this.GetDetailsForVideoGame(item.VideoGameId);

                if (game != null)
                {
                    if (game.Quantity < item.Quantity)
                    {
                        return false;
                    }

                    VideoGameInShoppingCart itemToAdd = new VideoGameInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        VideoGame = game,
                        VideoGameId = game.Id,
                        UserCart = userShoppingCard,
                        CartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };

                    game.Quantity = game.Quantity - item.Quantity;
                    this._videoGameRepository.Update(game);
                    this._videoGameInShoppingCartRepository.Insert(itemToAdd);
                    
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewVideoGame(VideoGame game)
        {
            this._videoGameRepository.Insert(game);
        }

        public void DeleteVideoGame(Guid id)
        {
           var game = this.GetDetailsForVideoGame(id);
           this._videoGameRepository.Delete(game);
        }

        public byte[] ExportAllVideoGames()
        {
            var result = _videoGameRepository.GetAll().ToList();
            return exportGamesXlsx(result);
        }

        public byte[] ExportVideoGamesByGenre(GenreEnum genre)
        {
            var result = _videoGameRepository.GetAll().Where(t => t.Genre.Equals(genre)).ToList();
            return exportGamesXlsx(result);
        }

        public List<VideoGame> FilterVideoGamesByGenre(GenreEnum genre)
        {
            var games = this._videoGameRepository.GetAll().Where(g =>
            {
                return g.Genre.Equals(genre) && g.Quantity > 0;
            });

            return games.ToList();
        }

        public List<VideoGame> GetAllVideoGames()
        {
            return this._videoGameRepository.GetAll().Where(g => g.Quantity > 0).ToList();
        }

        public VideoGame GetDetailsForVideoGame(Guid? id)
        {
            return this._videoGameRepository.Get(id);
        }

        public AddToCartDTO GetShoppingCartInfo(Guid? id)
        {
            var game = this.GetDetailsForVideoGame(id);
            AddToCartDTO model = new AddToCartDTO
            {
                SelectedVideoGame = game,
                VideoGameId = game.Id,
                Quantity = 1
            };

            return model;
        }

        public void UpdeteExistingVideoGame(VideoGame game)
        {
            this._videoGameRepository.Update(game);
        }

        private byte[] exportGamesXlsx(List<VideoGame> result)
        {
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Video Games");

                worksheet.Cell(1, 1).Value = "Video Game Id";
                worksheet.Cell(1, 2).Value = "Video Game Title";
                worksheet.Cell(1, 3).Value = "Video Game Genre";
                worksheet.Cell(1, 4).Value = "Video Game Description";
                worksheet.Cell(1, 5).Value = "Video Game Price";
                worksheet.Cell(1, 6).Value = "In Stock";

                for (int i = 1; i <= result.Count; i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.GameTitle;
                    worksheet.Cell(i + 1, 3).Value = item.Genre.ToString();
                    worksheet.Cell(i + 1, 4).Value = item.Description.ToString();
                    worksheet.Cell(i + 1, 5).Value = item.Price;
                    worksheet.Cell(i + 1, 6).Value = item.Quantity;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }

            }
        }
    }
}
