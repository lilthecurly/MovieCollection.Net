using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCollection.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Год обязателен")]
        [Range(1900, 2050)]
        public int Year { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 1000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
