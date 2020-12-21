using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MoviesAPI.Services
{
    //service to start executing logic in DoWork method (in this case changing InTheaters field due to year comparison)
    //according to TimeSpan argument(in this case once a 24 hour)
    public class MovieInTheaterService : IHostedService, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public MovieInTheaterService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var thisYear = DateTime.Today.Year;
                var movies = await context.Movies.Where(x => x.ReleaseDate.Year == thisYear).ToListAsync();
                if (movies.Any())
                {
                    foreach (var movie in movies)
                    {
                        movie.InTheaters = true;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
