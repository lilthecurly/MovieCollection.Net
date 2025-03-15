using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Data;
using MovieCollection.Models;

namespace MovieCollection.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _context;

        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Movie
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            // Добавление тестового фильма, если база пуста
            if (!_context.Movies.Any())
            {
                // Проверяем наличие категорий
                if (!_context.Categories.Any())
                {
                    // Создаем тестовую категорию
                    var testCategory = new Category { Name = "Тестовая категория" };
                    _context.Categories.Add(testCategory);
                    await _context.SaveChangesAsync();
                }

                // Создаем тестовый фильм
                var testMovie = new Movie
                {
                    Title = "Тестовый фильм",
                    Description = "Это тестовое описание",
                    Year = 2023,
                    Price = 9.99m,
                    CategoryId = _context.Categories.First().Id // Берем первую категорию
                };

                _context.Movies.Add(testMovie);
                await _context.SaveChangesAsync();
            }

            // Основная логика метода
            var movies = _context.Movies
                .Include(m => m.Category)
                .OrderBy(m => m.Title)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
            }

            var totalItems = await movies.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedMovies = await movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;

            return View(pagedMovies);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            // Загрузка категорий из базы данных
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                // Если есть ошибки валидации, загрузите категории заново
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка: {ex.Message}");
                ModelState.AddModelError("", "Не удалось сохранить фильм. Проверьте данные.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                movie.CategoryId
            );

            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                ModelState.AddModelError("", "Не удалось обновить фильм.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
