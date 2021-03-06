using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Models;

namespace RecipeProject.Controllers
{
    public class StepController : Controller
    {
        private readonly DataBaseContext _context;

        public StepController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: Step
        public async Task<IActionResult> Index()
        {
            var dataBaseContext = _context.Steps.Include(s => s.Recipe).Include(s => s.User);
            return View(await dataBaseContext.ToListAsync());
        }

        // GET: Step/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var step = await _context.Steps
                .Include(s => s.Recipe)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (step == null)
            {
                return NotFound();
            }

            return View(step);
        }

        // GET: Step/Create
        public IActionResult Create(int recipeId)
        {
            ViewData["RecipeId"] = recipeId;
            ViewData["UserId"] = Convert.ToInt32(User.FindFirst("Id").Value);
            return View();
        }

        // POST: Step/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Step step)
        {

            //Step step = new Step();
            //step.RecipeId = userRecipe.RecipeId;
            //step.Name = 
            step.UserId = Convert.ToInt32(User.FindFirst("Id").Value);
            //userRecipe.Steps.Add(step);
            if (ModelState.IsValid)
            {
                _context.Add(step);
                await _context.SaveChangesAsync();
                return Redirect("~/Recipe/Details/" + step.RecipeId);
            }
            //ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Id", step.RecipeId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", step.UserId);

            return RedirectToAction("Index", "Recipe", new { Page = 1 });
        }

        // GET: Step/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var step = await _context.Steps.FindAsync(id);
            if (step == null)
            {
                return NotFound();
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Id", step.RecipeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", step.UserId);
            return View(step);
        }

        // POST: Step/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId,RecipeId")] Step step)
        {
            if (id != step.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(step);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StepExists(step.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Id", step.RecipeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", step.UserId);
            return View(step);
        }

        // GET: Step/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var step = await _context.Steps
                .Include(s => s.Recipe)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (step == null)
            {
                return NotFound();
            }

            return View(step);
        }

        // POST: Step/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var step = await _context.Steps.FindAsync(id);
            _context.Steps.Remove(step);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StepExists(int id)
        {
            return _context.Steps.Any(e => e.Id == id);
        }
    }
}
