using System.ComponentModel.DataAnnotations;

namespace MovieCollection.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }

        [Range(1900, 2050, ErrorMessage = "Год должен быть между 1900 и 2050")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
