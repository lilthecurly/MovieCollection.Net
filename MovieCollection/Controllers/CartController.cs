using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Data;
using MovieCollection.Models;
using System.Security.Claims;

namespace MovieCollection.Controllers
{
    [Authorize] // Только для авторизованных пользователей
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID текущего пользователя
            var cartItems = await _context.CartItems
                .Include(c => c.Movie) // Подгружаем данные о фильме
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems); // Передаем список элементов корзины в представление
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int movieId)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound(); // Если фильм не найден
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Создаем новый элемент корзины
            var cartItem = new CartItem
            {
                MovieId = movieId,
                UserId = userId,
                Quantity = 1
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Movie"); // Возвращаемся к списку фильмов
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Возвращаемся в корзину
        }
    }
}