using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Filters;
using MoviesAPI.Services;

namespace MoviesAPI
{
    public class Startup
    {
        // IConfiguration configuration object serves as abstraction on appsettings.json
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers(options => { options.Filters.Add(typeof(MyExceptionFilter)); })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();

            services.AddAutoMapper(typeof(Startup));

            //configure service for storing images in Azure store
            services.AddTransient<IFileStorageService, AzureStorageService>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                        ClockSkew = TimeSpan.Zero
                    });

            services.AddTransient<IHostedService, MovieInTheaterService>();

            //configure service for storing images in local store (wwwroot directory)
            //services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();


            // AddSingleton allows to get the only one instance of InMemoryRepository
            // every time referring to service
            //services.AddSingleton<IRepository, InMemoryRepository>();

            // AddTransient allows to get new instance of InMemoryRepository every time referring to service
            //(so there can be many instances)
            //services.AddTransient<IRepository, InMemoryRepository>(); 

            // AddScoped allows to get instance of InMemoryRepository with lifetime within same HTTP request
            //services.AddScoped<IRepository, InMemoryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            //using middleware for storing images localy
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
