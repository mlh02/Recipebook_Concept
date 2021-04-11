using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
using RecipeProject.Models;

namespace RecipeProject.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class RecipeController : Controller
    {
        private readonly DataBaseContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _environment;
        [Obsolete]
        public RecipeController(DataBaseContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _environment = hostingEnvironment;

        }

        // GET: Recipe
        //public IActionResult Index()
        //{
        //    var dataBaseContext = _context.Recipes.Include(r => r.User);


        //    var q = (from r in _context.Recipes
        //                                            join u in _context.Users on r.UserId equals u.Id
        //                                            select new UserRecipe
        //                                            {
        //                                                RecipeId = r.Id,
        //                                                RecipeName = r.Name,
        //                                                RecipeImage = r.Image,
        //                                                RecipeDescription = r.Description,
        //                                                RecipeTimeToComplete = r.TimeToComplete,
        //                                                Image = u.Image,
        //                                                Name = u.Name,
        //                                                Id = u.Id
        //                                            });

        //    return View(q.ToList());
        //}

        [Route("recipe/{Page}")]   // recipe/1
        public IActionResult Index(int Page)
        {
            int Total = _context.Recipes.Count();
            int PageSize = 3;

            ViewBag.Total = Convert.ToInt32(Math.Ceiling((double)Total / (double)PageSize));
            if (Page != 1)
            {
                ViewBag.Previous = Page - 1;
            }
            else
            {
                ViewBag.Previous = null;
            }
            if (Page < Total)
            {
                ViewBag.Next = Page + 1;
            }
            else
            {
                ViewBag.Next = null;
            }


            int Skip = PageSize * (Page - 1);

            List<UserRecipe> q = (List<UserRecipe>)(from r in _context.Recipes
                                                            join u in _context.Users on r.UserId equals u.Id
                                                            select new UserRecipe
                                                            {
                                                                RecipeId = r.Id,
                                                                RecipeName = r.Name,
                                                                RecipeImage = r.Image,
                                                                RecipeDescription = r.Description,
                                                                RecipeTimeToComplete = r.TimeToComplete,
                                                                Image = u.Image,
                                                                Name = u.Name,
                                                                Id = u.Id
                                                            }).ToList();

            return View(q.AsQueryable().Skip(Skip).Take(PageSize).ToList());

        }


        // GET: Recipe/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserRecipe recipe = (UserRecipe)(from r in _context.Recipes
                                                     join u in _context.Users on r.UserId equals u.Id
                                                     where r.Id == id
                                                     select new UserRecipe
                                                     {
                                                         RecipeId = r.Id,
                                                         RecipeName = r.Name,
                                                         RecipeImage = r.Image,
                                                         RecipeDescription = r.Description,
                                                         RecipeTimeToComplete = r.TimeToComplete,
                                                         Image = u.Image,
                                                         Name = u.Name,
                                                         Id = u.Id,
                                                         Steps = _context.Steps.Where(f => f.RecipeId == r.Id).ToList(),
                                                         Ingredients = _context.Ingredients.Where(f => f.RecipeId == r.Id).ToList(),
                                                         Comments = _context.Comments.Where(f => f.RecipeId == r.Id).ToList()
                                                     }).FirstOrDefault();


            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }
        // GET: Recipe/Create
        public IActionResult Create()
        {
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Recipe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRecipe recipeView)
        {
            string fileName = string.Empty;
            string path = string.Empty;
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                var extension = Path.GetExtension(files[0].FileName);
                fileName = Guid.NewGuid().ToString() + extension;
                path = Path.Combine(_environment.WebRootPath, "RecipeImages") + "/" + fileName;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    files[0].CopyTo(fs);
                    fs.Flush();
                }
                recipeView.RecipeImage = fileName;
            }


            Recipe recipe = new Recipe();
            recipe.Description = recipeView.RecipeDescription;
            recipe.Name = recipeView.RecipeName;
            recipe.TimeToComplete = recipeView.RecipeTimeToComplete;
            recipe.Image = recipeView.RecipeImage;
            recipe.UserId = Convert.ToInt32(User.FindFirst("Id").Value);

            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Recipe", new { Page = 1 });
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return RedirectToAction("Index", "Recipe", new { Page = 1 });
        }

        // GET: Recipe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // POST: Recipe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,TimeToComplete,UserId")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View(recipe);
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return RedirectToAction("Index", "Recipe", new { Page = 1 });
        }

        // GET: Recipe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Recipe", new { Page = 1 });
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
