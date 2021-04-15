using System;
using Microsoft.EntityFrameworkCore;

namespace RecipeProject.Models
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Book> Books { get; set; }


    }

}
