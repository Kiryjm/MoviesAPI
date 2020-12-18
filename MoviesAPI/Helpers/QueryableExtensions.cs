using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.DTOs;

namespace MoviesAPI.Helpers
{
    public static class QueryableExtensions
    {
        //return {RecordsPerPage} amount of objects with same {RecordsPerPage} step
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage);
        }
    }
}
