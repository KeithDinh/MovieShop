using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(MovieShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Movie> GetPurchasedMovieById(int movieId, int userId)
        {
            var purchase = await _dbContext.Purchases
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(p => p.MovieId == movieId && p.UserId == userId);
            return purchase == null ? null : purchase.Movie;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<IEnumerable<Review>> GetUserReviews(int userId)
        {
            var reviews = await _dbContext
                .Reviews.Include(m => m.Movie)
                .Where(r => r.UserId == userId)
                .ToListAsync();
            return reviews;
        }
    }
}
