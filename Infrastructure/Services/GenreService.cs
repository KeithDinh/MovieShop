using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<List<GenreModel>> GetAllGenres()
        {
            var genres = await _genreRepository.ListAllAsync();
            var genreList = new List<GenreModel>();
            foreach (var genre in genres)
            {
                genreList.Add(new GenreModel()
                {
                    Id = genre.Id,
                    Name = genre.Name,
                });
            }
            return genreList;
        }

        public async Task<GenreDetailsResponseModel> GetGenreDetails(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            var genreMovies = new GenreDetailsResponseModel()
            {
                Id = genre.Id,
                Name = genre.Name,
            };

            genreMovies.Movies = new List<MovieCardResponseModel>();

            foreach(var movie in genre.Movies)
            {
                genreMovies.Movies.Add( new MovieCardResponseModel() {
                    Id = movie.Id,
                    Title = movie.Title,
                    PosterUrl = movie.PosterUrl,
                    Budget = movie.Budget.GetValueOrDefault(),
                });
            }

            return genreMovies;
        }
    }
}
