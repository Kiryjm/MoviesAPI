using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Helpers;

namespace MoviesAPI.Tests
{
    public class BaseTests
    {
        protected ApplicationDbContext BuildContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName).Options;

            var dbcontext = new ApplicationDbContext(options);
            return dbcontext;
        }

        protected IMapper BuildMap()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfiles());
            });
            return config.CreateMapper();
        }
    }
}
