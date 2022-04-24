using GemBox.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IGames.Domain.DomainModels;
using IGames.Repository.Interface;
using IGames.Services.Interface;

namespace IGames.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public List<Order> getAllOrders()
        {
            return this._orderRepository.getAllOrders();
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return this._orderRepository.getOrderDetails(model);
        }

        public DocumentModel CreateOrderInvoice(Guid orderId)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "InvoiceTemplate.docx");
            var document = DocumentModel.Load(templatePath);
            var order = this.getOrderDetails(new BaseEntity { Id = orderId});

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{OrderDate}}", order.DateOfOrder.ToString());
            document.Content.Replace("{{ClientEmail}}", order.ApplicationUser.Email);

            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;
            var numGames = 0;
            foreach (var game in order.VideoGamesInOrder)
            {
                totalPrice += game.Game.Price * game.Quantity;
                sb.Append("Video Game: " + game.Game.GameTitle);
                sb.Append("\tPrice: " + game.Game.Price.ToString() + "\n");
                numGames += game.Quantity;
            }

            document.Content.Replace("{{GameList}}", sb.ToString());
            document.Content.Replace("{{NumGamesOrdered}}", numGames.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString());

            return document;
        }
    }
}