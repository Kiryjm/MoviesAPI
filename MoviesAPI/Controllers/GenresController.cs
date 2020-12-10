using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IRepository repository;

        public GenresController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] //api/genres
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allgenres")] //replacing api/genres/list with allgenres
        //api/genres
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return await repository.GetAllGenres();
        }

        [HttpGet("{Id:int}", Name = "getGenre")] //api/genres/1
        public ActionResult<Genre> Get(int Id, string param2)
        {
            var genre = repository.GetGenreById(Id);
            if (genre == null)
            {
                return NotFound();
            }
            return genre;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {
            repository.AddGenre(genre);

            return new CreatedAtRouteResult("getGenre", new { Id = genre.Id }, genre);
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            return NoContent();

        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();

        }
    }
}
