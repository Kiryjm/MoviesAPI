using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        protected ControllerContext BuildControllerContextWithDefaultUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example@hotmail.com"),
                new Claim(ClaimTypes.Email, "example@hotmail.com"),
            }, "test"));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() {User = user}
            };
        }
    }
}
