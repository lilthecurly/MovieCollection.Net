using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Data;
using MovieCollection.Models;
using MovieCollection.Services;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace MovieCollection.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly StripeIntegrationService _stripeService;
        private readonly IConfiguration _config;

        public CartController(
            AppDbContext context,
            StripeIntegrationService stripeService,
            IConfiguration config)
        {
            _context = context;
            _stripeService = stripeService;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItems
                .Include(c => c.Movie)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            ViewBag.StripePublishableKey = _config["Stripe:PublishableKey"];
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int movieId)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null) return NotFound();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.MovieId == movieId && c.UserId == userId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        MovieId = movieId,
                        UserId = userId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity++;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Movie");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int movieId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.MovieId == movieId && c.UserId == userId);

                if (cartItem == null) return NotFound();

                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = await _context.CartItems
                    .Include(c => c.Movie)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest("Cart is empty");
                }

                // Проверка на null для URL
                var successUrl = Url.Action("PaymentSuccess", "Cart", null, Request.Scheme)
                    ?? throw new InvalidOperationException("Success URL is null");
                var cancelUrl = Url.Action("Index", "Cart", null, Request.Scheme)
                    ?? throw new InvalidOperationException("Cancel URL is null");

                var session = _stripeService.CreateCheckoutSession(
                    cartItems,
                    successUrl,
                    cancelUrl
                );

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Json(new { id = session.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }
    }
}