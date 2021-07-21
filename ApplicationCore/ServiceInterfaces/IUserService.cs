using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IUserService
    {
        Task<UserRegisterResponseModel> RegisterUser(UserRegisterRequestModel requestModel);
        Task<UserLoginResponseModel> Login(string email, string password);
        Task<MovieCardResponseModel> BuyMovie(int movieId);
        Task<UserResponseModel> GetUserById(int id);
        Task<string> AddToFavorite(int movieId);
        Task<string> RemoveFromFavorite(int movieId);
    }
}