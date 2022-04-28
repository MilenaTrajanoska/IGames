using IGames.Domain.DomainModels;
using IGames.Domain.DTO;
using IGames.Repository.Interface;
using IGames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGames.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private IRepository<ShoppingCart> _shoppingCartRepository;
        private IUserRepository _userRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<VideoGameInOrder> _videoGamesInOrderRepository;
        private IRepository<VideoGame> _videoGameRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<VideoGameInOrder> videoGamesInOrderRepository, IRepository<VideoGame> videoGameRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _videoGamesInOrderRepository = videoGamesInOrderRepository;
            _videoGameRepository = videoGameRepository;
        }
        public bool deleteVideoGameFromShoppingCart(string userId, Guid gameId)
        {
            if (!string.IsNullOrEmpty(userId) && gameId != null)
            {
                var user = this._userRepository.Get(userId);
                var userCart = user.UserCart;
                var gameToDelete = userCart.VideoGamesInShoppingCart.Where(g => g.VideoGame.Id.Equals(gameId)).FirstOrDefault();
                userCart.VideoGamesInShoppingCart.Remove(gameToDelete);
                var gameToUpdate = gameToDelete.VideoGame;
                gameToUpdate.Quantity += gameToDelete.Quantity;
                this._shoppingCartRepository.Update(userCart);
                this._videoGameRepository.Update(gameToUpdate);
                return true;
            }
            return false;
        }

        public CartDTO getShoppingCartInfo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Invalid user id!");

            var user = this._userRepository.Get(userId);
            var userCart = user.UserCart;
            var games = userCart.VideoGamesInShoppingCart.ToList();

            double totalPrice = 0.0;

            foreach (var game in games)
            {
                totalPrice += game.Quantity * game.VideoGame.Price;
            }

            var shoppingCartDto = new CartDTO
            {
                Games = games,
                TotalPrice = totalPrice
            };

            return shoppingCartDto;
        }

        public bool orderNow(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Invalid user id!");

            var user = this._userRepository.Get(userId);

            var userShoppingCart = user.UserCart;

            Order order = new Order
            {
                Id = Guid.NewGuid(),
                ApplicationUser = user,
                UserId = userId,
                DateOfOrder = DateTime.Now
            };

            this._orderRepository.Insert(order);

            List<VideoGameInOrder> gamesToBeOrdered = new List<VideoGameInOrder>();

            var orderedGames = userShoppingCart.VideoGamesInShoppingCart.Select(z => new VideoGameInOrder
            {
                Id = Guid.NewGuid(),
                VideoGameId = z.VideoGame.Id,
                Game = z.VideoGame,
                OrderId = order.Id,
                order = order,
                Quantity = z.Quantity
            }).ToList();

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            sb.AppendLine("Your order has been successfully completed. The order conains: ");

            for (int i = 1; i <= orderedGames.Count(); i++)
            {
                var oGame = orderedGames[i - 1];

                totalPrice += oGame.Quantity * oGame.Game.Price;

                sb.AppendLine(i.ToString() + ". Video Game Details: \n Game Title: " +
                    oGame.Game.GameTitle + "\n Genre: " +
                    oGame.Game.Genre + "\n Description: " +
                    oGame.Game.Description + "\n Price: " +
                    oGame.Game.Price + "Quantity: " +
                    + oGame.Quantity);
            }

            sb.AppendLine("Total price: " + totalPrice.ToString());


            gamesToBeOrdered.AddRange(orderedGames);

            foreach (var item in gamesToBeOrdered)
            {
                this._videoGamesInOrderRepository.Insert(item);
            }

            user.UserCart.VideoGamesInShoppingCart.Clear();

            this._userRepository.Update(user);

            return true;
        }
    }
}
