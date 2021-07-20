using ApplicationCore.ServiceInterfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShopMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICurrentUser _currentUser;

        public UsersController(IUserService userService, ICurrentUser currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
        }
        [HttpPost]
        public async Task<IActionResult> BuyMovie(int id)
        {
            if (!_currentUser.isAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var movie = await _userService.BuyMovie(id);
            return View("TransactionComplete", movie);
        }
    }
}
