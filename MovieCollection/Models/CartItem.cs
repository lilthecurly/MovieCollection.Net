namespace MovieCollection.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int Quantity { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
