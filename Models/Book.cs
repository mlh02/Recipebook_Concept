using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeProject.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public int ISBM { get; set; }


    }
}
