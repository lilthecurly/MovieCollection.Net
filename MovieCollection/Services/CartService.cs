using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MovieCollection.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace MovieCollection.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<CartItem>();

            var cartJson = session.GetString("Cart");
            return cartJson == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        public void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString("Cart", JsonSerializer.Serialize(cart));
            }
        }
    }
}