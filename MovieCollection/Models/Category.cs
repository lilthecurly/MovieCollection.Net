using System.ComponentModel.DataAnnotations;

namespace MovieCollection.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        public string Name { get; set; }

        public List<Movie>? Movies { get; set; }
    }
}
