using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    public class CustomBaseController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity: class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO pagination) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsNoTracking().AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);
            var entities = await queryable.Paginate(pagination).ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(id, queryable);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id, IQueryable<TEntity> queryable ) where TEntity : class, IId
        {
            var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return mapper.Map<TDTO>(entity);
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation creation, string routeName) where TEntity: class, IId
        {
            var entity = mapper.Map<TEntity>(creation);
            context.Add(entity);
            await context.SaveChangesAsync();
            var readDTO = mapper.Map<GenreDTO>(entity);

            //return location of created resource: route name to access tne newly created resource, 
            //route values of action with such route name and created object itself
            return new CreatedAtRouteResult(routeName, new { readDTO.Id }, readDTO);
        }

        protected async Task<ActionResult> Put<TCreation, TEntity>(int id, [FromBody] TCreation creation) where TEntity: class, IId
        {
            var entity = mapper.Map<TEntity>(creation);
            entity.Id = id;

            //Indication of modification existing in db resource genre
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();

        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var exists = await context.Set<TEntity>().AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new TEntity { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, [FromBody] JsonPatchDocument<TDTO> patchDocument) where TDTO: class
            where TEntity: class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<TDTO>(entityFromDB);

            //apply changes from json document to the entityDTO
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);
            if (!isValid)
            {
                //passing ModelState to indicate occured validation errors
                return BadRequest(ModelState);
            }

            mapper.Map(entityDTO, entityFromDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
