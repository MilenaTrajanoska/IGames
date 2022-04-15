using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using IGames.Services.Interface;

namespace TicketShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {


        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }


        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }

        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.deleteVideoGameFromShoppingCart(userId, id);
            
            if (!result)
            {
                ViewData["ErrorMessage"] = "Could not remove item form shopping cart! <br> Please try again.";
            }
            else
            {
                ViewData["SuccessMessage"] = "You have successfully removed the item from your shopping cart!";
            }

            return View("Index", this._shoppingCartService.getShoppingCartInfo(userId));

        }

        [Authorize]
        public IActionResult PaymentForOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "IGames Shop Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.SaveOrderToDB();

                if (result)
                {
                    ViewData["Success"] = "Successfully completed order.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Error"] = "Could not save your order to the database.";
                    return RedirectToAction("Index");
                }
            }

            ViewData["Error"] = "Could not process payment requ.";
            return RedirectToAction("Index");
        }

        private Boolean SaveOrderToDB()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);

            return result;
        }
    }
}
