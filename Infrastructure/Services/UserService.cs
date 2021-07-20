﻿using ApplicationCore.Entities;
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
        public UserService(IUserRepository userRepository, ICurrentUser currentUser, IPurchaseRepository purchaseRepository, IMovieRepository movieRepository)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _purchaseRepository = purchaseRepository;
            _movieRespository = movieRepository;
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
    }
}
