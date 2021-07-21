using ApplicationCore.ServiceInterfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUser _currentUser;

        public UserController(IUserService userService, ICurrentUser currenUser)
        {
            _userService = userService;
            _currentUser = currenUser;
        }
        [HttpPost]
        [Route("purchase")]
        public async Task<IActionResult> BuyMovie(int id)
        {
            if (!_currentUser.isAuthenticated)
            {
                return Unauthorized("Need to login first");
            }
            var movie = await _userService.BuyMovie(id);
            return Ok(movie);
        }
        [HttpPost]
        [Route("favorite")]
        public async Task<IActionResult> Favorite([FromBody] int movieId)
        {
            var result = await _userService.AddToFavorite(movieId);
            return Ok(result);
        }
        [HttpPost]
        [Route("unfavorite")]
        public async Task<IActionResult> UnFavorite([FromBody] int movieId)
        {
            var result = await _userService.RemoveFromFavorite(movieId);
            return Ok(result);
        }
    }
}
