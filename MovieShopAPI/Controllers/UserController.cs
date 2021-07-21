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

        public UserController(IUserService userService)
        {
            _userService = userService;
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
