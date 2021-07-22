using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMovieRepository _movieRespository;
        private readonly IFavoriteRepository _favoriteRepository;
        public UserService(IUserRepository userRepository, ICurrentUser currentUser, IPurchaseRepository purchaseRepository, IMovieRepository movieRepository, IFavoriteRepository favoriteRepository)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _purchaseRepository = purchaseRepository;
            _movieRespository = movieRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<string> AddToFavorite(int movieId)
        {
            var dbFavorite = await _favoriteRepository.GetExistAsync(f => f.MovieId == movieId && f.UserId == _currentUser.UserId);
            if(dbFavorite != true)
            {
                return "Conflict";
            }

            await _favoriteRepository.AddAsync(new Favorite { 
                UserId = _currentUser.UserId,
                MovieId = movieId
            });
            return "Added";
        }

        public async Task<string> RemoveFromFavorite(int movieId)
        {
            var dbFavorite = await _favoriteRepository.ListAsync(f => f.MovieId == movieId && f.UserId == _currentUser.UserId);
            if (dbFavorite.Count() == 1)
            {
                
                await _favoriteRepository.DeleteAsync(dbFavorite.ToList()[0]);
                return "Removed";
            }
            return "Nothing to remove";
        }
        public async Task<MovieCardResponseModel> BuyMovie(int movieId)
        {
            var dbPurchasedMovie = await _userRepository.GetPurchasedMovieById(movieId,_currentUser.UserId);
            if (dbPurchasedMovie != null)
            {
                throw new ConflictException("You already bought this product");
            }
            var movie = await _movieRespository.GetByIdAsync(movieId);
            var newPurchase = new Purchase
            {
                UserId = _currentUser.UserId,
                TotalPrice = movie.Price.GetValueOrDefault(),
                PurchaseDateTime = DateTime.Now,
                MovieId = movie.Id,
            };
            var createdPurchase = await _purchaseRepository.AddAsync(newPurchase);

            return new MovieCardResponseModel
            {
                Id = movie.Id,
                Budget = movie.Budget.GetValueOrDefault(),
                PosterUrl = movie.PosterUrl,
                Title = movie.Title
            };
        }

        public async Task<UserResponseModel> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
            };
        }

        public async Task<UserLoginResponseModel> Login(string email, string password)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            if (dbUser == null)
            {
                throw new NotFoundException("Email does not exist. Please register first");
            }

            var hashedPassword = HashPassword(password, dbUser.Salt);
            if(hashedPassword == dbUser.HashedPassword)
            {
                var userLoginResponse = new UserLoginResponseModel
                {
                    Id = dbUser.Id,
                    Email = dbUser.Email,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                };
                return userLoginResponse;
            }
            return null;
        }

        public async Task<UserRegisterResponseModel> RegisterUser(UserRegisterRequestModel requestModel)
        {
            var dbUser = await _userRepository.GetUserByEmail(requestModel.Email);
            if(dbUser != null)
            {
                throw new ConflictException("Email already exists");
            }

            var salt = CreateSalt();
            var hashedPassword = HashPassword(requestModel.Password, salt);

            var user = new User
            {
                Email = requestModel.Email,
                Salt = salt,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                HashedPassword = hashedPassword
            };

            // add user dbcontext + database
            var createdUser = await _userRepository.AddAsync(user);

            var userResponse = new UserRegisterResponseModel
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
            };

            return userResponse;
        }

        private string CreateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private string HashPassword(string password, string salt)
        {
            // Aarogon
            // Pbkdf2
            // BCrypt
            var hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Convert.FromBase64String(salt),
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
            return hashed;
        }

        public async Task<List<ReviewModel>> GetUserReviews(int movieId)
        {
            var dbReviews = await _movieRespository.GetMovieReviews(movieId);
            var reviews = new List<ReviewModel>();
            foreach (var review in dbReviews)
            {
                reviews.Add(new ReviewModel
                {
                    MovieId = review.MovieId,
                    UserId = review.UserId,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                });
            }
            return reviews;
        }
        public async Task<List<MovieCardResponseModel>> GetUserFavoriteMovies(int userId)
        {
            var dbMovies = await _userRepository.GetUserFavoriteMovies(userId);
            var movies = new List<MovieCardResponseModel>();
            foreach (var movie in dbMovies)
            {
                movies.Add(new MovieCardResponseModel
                {
                    Id = movie.Id,
                    Budget = movie.Budget.GetValueOrDefault(),
                    PosterUrl = movie.PosterUrl,
                    Title = movie.Title
                });
            }
            return movies;
        }
        public async Task<List<MovieCardResponseModel>> GetUserPurchases(int userId)
        {
            var dbMovies = await _userRepository.GetUserPurchases(userId);
            var movies = new List<MovieCardResponseModel>();
            foreach (var movie in dbMovies)
            {
                movies.Add(new MovieCardResponseModel
                {
                    Id = movie.Id,
                    Budget = movie.Budget.GetValueOrDefault(),
                    PosterUrl = movie.PosterUrl,
                    Title = movie.Title
                });
            }
            return movies;
        }
    }
}
