using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "people";

        public PeopleController(ApplicationDbContext context, IMapper mapper, 
            IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get()
        {
            var people = await context.People.ToListAsync();
            return mapper.Map<List<PersonDTO>>(people);
        }

        [HttpGet("{id}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id ==id);
            if (person == null)
            {
                return NotFound();
            }

            return mapper.Map<PersonDTO>(person);
        }

        [HttpPost]
        public async Task<ActionResult<PersonDTO>> Post([FromForm] PersonCreationDTO personCreation)
        {
            var person = mapper.Map<Person>(personCreation);

            if (personCreation.Picture != null)
            {
                //Representing file as byte array
                using (var memoryStream = new MemoryStream())
                {
                    await personCreation.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    //var extension = personCreation.Picture.FileName.Split(".")[1];
                    var extension = Path.GetExtension(personCreation.Picture.FileName);
                    person.Picture =
                        await fileStorageService.SaveFile(content, extension, containerName, personCreation.Picture.ContentType);
                }
            }
            
            context.Add(person);
            await context.SaveChangesAsync();
            var personDTO = mapper.Map<PersonDTO>(person);

            //Return location of created resource: route name to access tne newly created resource,
            //route values of action with such route name and created object itself
            return new CreatedAtRouteResult("getPerson", new { person.Id }, personDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDTO personCreation)
        {
            var personDB = await context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (personDB == null)
            {
                return NotFound();
            }

            personDB = mapper.Map(personCreation, personDB);

            if (personCreation.Picture != null)
            {
                //Representing file as byte array
                using (var memoryStream = new MemoryStream())
                {
                    await personCreation.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreation.Picture.FileName);
                    personDB.Picture =
                        await fileStorageService.EditFile(content, extension, containerName, personDB.Picture, 
                            personCreation.Picture.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.People.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new Person { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
