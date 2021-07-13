using ApplicationCore.ServiceInterfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using MovieShopMVC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private IMovieService _movieService;
        public HomeController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index()
        {
            var movies = _movieService.GetTopRevenueMovies();

            // 3 ways to send the data from Controller/action to View
            // 1. Models (strongly typed models)
            // 2. ViewBag
            // 3. ViewData
            ViewBag.MoviesCount = movies.Count();

            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // GET localhost:5001/home/GetHighestGrossingMovies
        [HttpGet]
        public IActionResult GetHighestGrossingMovies()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
