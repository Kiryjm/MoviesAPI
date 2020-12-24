using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/people")]
    [EnableCors(PolicyName = "AllowAPIRequestIO")]
    public class PeopleController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "people";

        public PeopleController(ApplicationDbContext context, IMapper mapper, 
            IFileStorageService fileStorageService) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet(Name = "getPeople")]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery]PaginationDTO pagination)
        {
            return await Get<Person, PersonDTO>(pagination);
        }

        [HttpGet("{id}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            return await Get<Person, PersonDTO>(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            return await Patch<Person, PersonPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [DisableCors]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Person>(id);
        }
    }
}
