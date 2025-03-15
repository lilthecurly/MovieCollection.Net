using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCollection.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название фильма обязательно")]
        [StringLength(200, ErrorMessage = "Максимальная длина названия 200 символов")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Некорректный год выпуска")]
        public int Year { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? Poster { get; set; }
    }
}