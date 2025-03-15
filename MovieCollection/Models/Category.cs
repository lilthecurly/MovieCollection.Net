using System.ComponentModel.DataAnnotations;

namespace MovieCollection.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, ErrorMessage = "Максимальная длина названия 100 символов")]
        public string? Name { get; set; }

        public List<Movie> Movies { get; set; } = new();
    }
}