using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreationDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Person, PersonPatchDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));
            CreateMap<Movie, MoviePatchDTO>().ReverseMap();
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreation, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieCreation.GenresIds)
            {
                result.Add(new MoviesGenres { GenreId = id });
            }

            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreation, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieCreation.Actors)
            {
                result.Add(new MoviesActors { PersonId = actor.PersonId, Character = actor.Character });
            }

            return result;
        }
    }
}
