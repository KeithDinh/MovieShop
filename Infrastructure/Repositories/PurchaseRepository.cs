using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PurchaseRepository : EfRepository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(MovieShopDbContext dbContext) : base(dbContext) {}
        public bool FindPurchaseByUserMovie(int userId, int movieId)
        {
            return _dbContext.Purchases.Any(p => p.UserId == userId && p.MovieId == movieId);
        }
    }
}
