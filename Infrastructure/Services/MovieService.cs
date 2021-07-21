using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;

namespace Infrastructure.Services
{

    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IReviewRepository _reviewRepository;
        public MovieService(IMovieRepository movieRepository, IReviewRepository reviewRepository)
        {
            _movieRepository = movieRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<List<MovieCardResponseModel>> GetTopRevenueMovies()
        {
            var movies = await _movieRepository.GetHighest30GrossingMovies();
            var movieCards = new List<MovieCardResponseModel>();

            foreach (var movie in movies)
            {
                movieCards.Add(new MovieCardResponseModel 
                { 
                    Id = movie.Id, 
                    Budget = movie.Budget.GetValueOrDefault(), 
                    PosterUrl = movie.PosterUrl, 
                    Title = movie.Title 
                });
            }
            return movieCards;
        }

        public async Task<MovieDetailsResponseModel> GetMovieDetails(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            var movieDetails = new MovieDetailsResponseModel()
            {
                Id = movie.Id,
                Title = movie.Title,
                PosterUrl = movie.PosterUrl,
                BackdropUrl = movie.BackdropUrl,
                Rating = movie.Rating,
                Overview = movie.Overview,
                Tagline = movie.Tagline,
                Budget = movie.Budget.GetValueOrDefault(),
                ImdbUrl = movie.ImdbUrl,
                TmdbUrl = movie.TmdbUrl,
                ReleaseDate = movie.ReleaseDate,
                RunTime = movie.RunTime,
                Price = movie.Price,
                Revenue = movie.Revenue,
            };

            movieDetails.Casts = new List<CastResponseModel>();
            foreach (var cast in movie.MovieCasts)
            {
                movieDetails.Casts.Add(new CastResponseModel ()
                {
                    Id = cast.CastId,
                    Name = cast.Cast.Name,
                    Character = cast.Character,
                    ProfilePath = cast.Cast.ProfilePath,
                    TmdbUrl = cast.Cast.TmdbUrl,
                });
            }

            movieDetails.Genres = new List<GenreModel>();
            foreach (var genre in movie.Genres)
            {
                movieDetails.Genres.Add(
                    new GenreModel
                    {
                        Id = genre.Id,
                        Name = genre.Name
                    }
                );
            }
            return movieDetails;
        }

        public async Task<List<ReviewModel>> GetMovieReviews(int movieId)
        {
            var dbReviews = await _reviewRepository.ListAsync(r => r.MovieId == movieId);
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
        public async Task<List<MovieCardResponseModel>> GetTopRatedMovies()
        {
            var dbMovies =  await _movieRepository.GetTopRatedMovies();

            var movies = new List<MovieCardResponseModel>();
            foreach (var movie in dbMovies)
            {
                movies.Add(new MovieCardResponseModel
                {
                    Id = movie.Id,
                    Budget = movie.Budget.GetValueOrDefault(),
                    PosterUrl = movie.PosterUrl,
                    Title = movie.Title,
                    Rating = movie.Rating,
                });
            }
            return movies;
        }
    }
}