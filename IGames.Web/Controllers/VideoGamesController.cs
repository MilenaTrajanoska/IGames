using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IGames.Domain.DomainModels;
using IGames.Domain.DTO;
using IGames.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IGames.Web.Controllers
{
    public class VideoGamesController : Controller
    {
        private readonly IVideoGameService _videoGameService;

        private readonly List<SelectListItem> Genres = new List<SelectListItem>{
                    new SelectListItem  { Value = GenreEnum.ACTION.ToString(), Text = GenreEnum.ACTION.ToString()},
                    new SelectListItem  { Value = GenreEnum.COMEDY.ToString(), Text = GenreEnum.COMEDY.ToString() },
                    new SelectListItem  { Value = GenreEnum.DRAMA.ToString(), Text = GenreEnum.DRAMA.ToString() },
                    new SelectListItem  { Value = GenreEnum.HORROR.ToString(), Text = GenreEnum.HORROR.ToString() },
                    new SelectListItem  { Value = GenreEnum.KIDS.ToString(), Text = GenreEnum.KIDS.ToString() },
                    new SelectListItem  { Value = GenreEnum.MISTERY.ToString(), Text = GenreEnum.MISTERY.ToString() },
                    new SelectListItem  { Value = GenreEnum.ROMANCE.ToString(), Text = GenreEnum.ROMANCE.ToString() },
                    new SelectListItem  { Value = GenreEnum.SCI_FI.ToString(), Text = GenreEnum.SCI_FI.ToString() },
                    new SelectListItem  { Value = GenreEnum.THRILLER.ToString(), Text = GenreEnum.THRILLER.ToString() },
                };
        public VideoGamesController(IVideoGameService videoGameService)
        {
            _videoGameService = videoGameService;
        }

        // GET: VideoGames
        public IActionResult Index()
        {
            var allGames = this._videoGameService.GetAllVideoGames();
            ViewData["Genres"] = Genres;
            return View(allGames);
        }

        [Authorize]
        public IActionResult AddVideoGameToCart(Guid? id)
        {
            var model = this._videoGameService.GetShoppingCartInfo(id);
            ViewData["Genres"] = Genres;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddVideoGameToCart([Bind("VideoGameId", "Quantity")] AddToCartDTO item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var game = this._videoGameService.GetDetailsForVideoGame(item.VideoGameId);
            if (game.Quantity < item.Quantity)
            {
                ViewData["Error Message"] = "The available quantity for this video game in stock is "
                    + game.Quantity + ".\nPlease select an adequate amount to purchase.";
                return View(item);
            }

            var result = this._videoGameService.AddVideoGameToShoppingCart(item, userId);

            if (result)
            {
                return RedirectToAction("Index", "VideoGames");
            }

            return View(item);
        }

        // GET: VideoGames/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = this._videoGameService.GetDetailsForVideoGame(id);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: VideoGames/Create
        public IActionResult Create()
        {
            ViewData["Genres"] = Genres;
            return View();
        }

        // POST: VideoGames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create([Bind("Id,GameTitle,Image,Genre,Price,Quantity, Description")] VideoGame game)
        {
            if (ModelState.IsValid)
            {
                this._videoGameService.CreateNewVideoGame(game);
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var game = this._videoGameService.GetDetailsForVideoGame(Id);

            if (game == null)
            {
                return NotFound();
            }
            ViewData["Genres"] = Genres;
            return View(game);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,GameTitle,Image,Genre,Price,Quantity, Description")] VideoGame game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._videoGameService.UpdeteExistingVideoGame(game);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoGameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: VideoGames/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = this._videoGameService.GetDetailsForVideoGame(id);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: VideoGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._videoGameService.DeleteVideoGame(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("FilterTicketsByDate")]
        public IActionResult FilterVideoGamesByGenre(GenreEnum genre)
        {
            var games = _videoGameService.FilterVideoGamesByGenre(genre);
            if (games != null && games.Count != 0)
            {
                return View("Index", games);
            }
            ViewData["ErrorMessage"] = "No video games availbale for the specified genre. \n Displaying all video games.";
            return View("Index", _videoGameService.GetAllVideoGames());
        }

        [HttpGet]
        [Authorize(Roles = Role.ADMIN)]
        public FileContentResult ExportAllVideoGames()
        {
            string fileName = "VideoGames.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var content = _videoGameService.ExportAllVideoGames();
            return File(content, contentType, fileName);
        }

        private bool VideoGameExists(Guid id)
        {
            return this._videoGameService.GetDetailsForVideoGame(id) != null;
        }
    }
}
