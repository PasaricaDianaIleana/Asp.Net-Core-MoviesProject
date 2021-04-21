using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovies.Data;
using MvcMovies.Models;
using MvcMovies.ViewModel;

namespace MvcMovies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;

        public MoviesController(MvcMovieContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["Price"] = sortOrder == "Price" ? "price" : "Price";
            ViewData["Genre"] = sortOrder == "Genre" ? "genre" : "Genre";
            ViewData["CurrentFilter"] = searchString;
            ViewBag.movie = _context.Movies.ToList();
            var movie = from s in _context.Movies select s;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                movie = movie.Where(m => m.Title.Contains(searchString) || m.Genre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    movie = movie.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    movie = movie.OrderBy(s => s.ReleaseDate);
                    break;
                case "date_desc":
                    movie = movie.OrderByDescending(s => s.ReleaseDate);
                    break;
                case "price":
                    movie = movie.OrderBy(s => s.Price);
                    break;
                case "genre":
                    movie = movie.OrderBy(s => s.Genre);
                    break;
                default:
                    movie = movie.OrderBy(s => s.Title);
                    break;
            }



            int pageSize = 3;
            return View(await MvcMovies.PaginatedList<Movie>.CreateAsync(movie.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        public ActionResult AddReview(ReviewMovieViewModel review)
        {
            Reviews r = new Reviews();

            r.Name = review.Name;
            r.Comment= review.Comment;
            r.dateTime= review.Date;
            r.MovieId = review.MovieId;
            _context.Reviews.Add(r);
            _context.SaveChanges();
            return RedirectToAction($"Details/{review.MovieId}", "Movies");
        }
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var data = _context.Movies.Include(a => a.Reviews)
                .FirstOrDefault(m => m.MovieId == id);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
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
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
