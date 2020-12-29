using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;
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

            services.AddApplicationInsightsTelemetry();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                    sqlServer => sqlServer.UseNetTopologySuite()));

            //allows using certain origin to use cross resources requests with any headers
            services.AddCors(options => options.AddPolicy("AllowAPIRequestIO",
                builder => builder.WithOrigins("https://www.apirequest.io")
                    .WithMethods("GET", "POST").AllowAnyHeader()));

            //encrypting service
            services.AddDataProtection();

            services.AddControllers(options => { options.Filters.Add(typeof(MyExceptionFilter)); })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Description = "This is a Web API for Movies operations",
                    TermsOfService = new Uri("https://udemy.com/user/felipegaviln/"),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT"
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "Felipe Gavilan",
                        Email = "felipe_gavilan887@hotmail.com",
                        Url = new Uri("https://gavilan.blog/")
                    }
                });

                //adding xml comments to the code
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper(typeof(Startup));

            //using hashing
            services.AddTransient<HashService>();

            //configure service for storing images in Azure store
            services.AddTransient<IFileStorageService, AzureStorageService>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.AddTransient<GenreHATEOASAttribute>();
            //services.AddTransient<LinksGenerator>();

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
            app.UseSwagger();

            //user can access visual documentation to explore endpoints of app
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI");
            });

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

            app.UseCors();
            //policy will be applied to the whole application (web api level)
            //app.UseCors(builder => 
            //    builder.WithOrigins("https://www.apirequest.io")
            //        .WithMethods("GET", "POST").AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
