using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class GenresController : ControllerBase
    {
        private ILogger<GenresController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger, 
            ApplicationDbContext context, IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getGenres")] //api/genres
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors(PolicyName = "AllowAPIRequestIO")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<IActionResult> Get()
        {
            var genres =  await context.Genres.AsNoTracking().ToListAsync();
            var genresDTOs = mapper.Map<List<GenreDTO>>(genres);

            //if (includeHATEOAS)
            //{
            //    var resourceCollection = new ResourceCollection<GenreDTO>(genresDTOs);
            //    genresDTOs.ForEach(genre => GenerateLinks(genre));
            //    resourceCollection.Links.Add(new Link(Url.Link("getGenres", new { }), rel: "self", method: "GET"));
            //    resourceCollection.Links.Add(new Link(Url.Link("createGenre", new { }), rel: "self", method: "POST"));
            //    return Ok(resourceCollection);
            //}

            return Ok(genresDTOs);
        }

        private void GenerateLinks(GenreDTO genreDTO)
        {
            genreDTO.Links.Add(new Link(Url.Link("getGenre", new { Id = genreDTO.Id }), "get-genre", method: "GET"));
            genreDTO.Links.Add(new Link(Url.Link("putGenre", new { Id = genreDTO.Id }), "put-genre", method: "PUT"));
            genreDTO.Links.Add(new Link(Url.Link("deleteGenre", new { Id = genreDTO.Id }), "delete-genre", method: "DELETE"));
        }

        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(GenreDTO), 200)]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        [HttpGet("{Id:int}", Name = "getGenre")] //api/genres/1
        public async Task<ActionResult<GenreDTO>> Get(int Id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre == null)
            {
                return NotFound();
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);

            //if (includeHATEOAS)
            //{
            //    GenerateLinks(genreDTO);
            //}

            return genreDTO;
        }

        [HttpPost(Name = "createGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<CreatedAtRouteResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            var genre = mapper.Map<Genre>(genreCreation);
            context.Add(genre);
            await context.SaveChangesAsync();
            var genreDTO = mapper.Map<GenreDTO>(genre);

            //return location of created resource: 
            //route name to access tne newly created resource, 
            //route values of action with such route name
            //and created object itself
            return new CreatedAtRouteResult("getGenre", new { genreDTO.Id }, genreDTO);
        }

        [HttpPut("{id}", Name = "putGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreation)
        {
            var genre = mapper.Map<Genre>(genreCreation);
            genre.Id = id;

            //Indication of modification existing in db resource genre
            context.Entry(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();

        }
        /// <summary>
        /// Delete a genre
        /// </summary>
        /// <param name="id"> id of the genre to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "deleteGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new Genre { Id = id });
            await context.SaveChangesAsync();

            return NoContent();

        }
    }
}
