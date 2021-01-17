using ANPaX.Backend.Models;
using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DTO;
using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using NLog;

namespace ANPaX.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddDbContext<DataContext>(options => options.UseSqlite(Configuration.GetConnectionString("SQLiteFile")));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ANPaX.Backend", Version = "v1" });
            });

            services.AddScoped<IDataStorageHelper<AggregateConfigurationDTO>, AggregateFormationConfigStorageHelper>();
            services.AddScoped<IDataStorageHelper<ParticleSimulationDTO>, ParticleSimulationStorageHelper>();
            services.AddSingleton<INeighborslistFactory, AccordNeighborslistFactory>();
            services.AddSingleton<ISimulationBoxFactory, AbsoluteTetragonalSimulationBoxFactory>();
            services.AddSingleton<ISingleParticleDepositionHandler, BallisticSingleParticleDepositionHandler>();
            services.AddSingleton<IAggregateDepositionHandler, BallisticAggregateDepositionHandler>();
            services.AddSingleton<IWallCollisionHandler, PeriodicBoundaryCollisionHandler>();
            services.AddSingleton<SimulationMonitor>();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            services.AddSingleton<ILogger>(logger);

            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ANPaX.Backend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
