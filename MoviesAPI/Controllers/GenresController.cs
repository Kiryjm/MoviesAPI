using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IRepository repository;
        private ILogger<GenresController> logger;

        public GenresController(IRepository repository, ILogger<GenresController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet] //api/genres
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allgenres")] //replacing api/genres/list with allgenres
        //api/genres
        public async Task<ActionResult<List<Genre>>> Get()
        {
            logger.LogInformation("Getting all the genres");
            return await repository.GetAllGenres();
        }

        [HttpGet("{Id:int}", Name = "getGenre")] //api/genres/1
        public ActionResult<Genre> Get(int Id, string param2)
        {
            logger.LogDebug("Get by Id method executing...");
            var genre = repository.GetGenreById(Id);
            if (genre == null)
            {
                logger.LogWarning($"Genre with Id {Id} not found");
                logger.LogError("This is an error");

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
