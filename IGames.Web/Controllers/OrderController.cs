using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IGames.Domain.DomainModels;
using IGames.Services.Interface;

namespace IGames.Web.Controllers
{
    public class OrderController : Controller
    {
        public IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userOrders = _orderService.getAllOrders().Where(o => o.UserId.Equals(userId)).ToList();
            return View(userOrders);
        }

        [Authorize]
        public FileContentResult CreateOrderInvoice(Guid orderId)
        {
           var document = _orderService.CreateOrderInvoice(orderId);
           var stream = new MemoryStream();

           document.Save(stream, new PdfSaveOptions());

           return File(stream.ToArray(), new PdfSaveOptions().ContentType, "Invoice-" + orderId.ToString() + ".pdf");
        }

        [Authorize]
        public IActionResult Details(Guid orderId)
        {
            var order = _orderService.getOrderDetails(new BaseEntity { Id = orderId });
            return View(order);
        }

    }
}