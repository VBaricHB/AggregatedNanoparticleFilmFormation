
using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.interfaces;
using ANPaX.UI.BlazorServer.Model;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Syncfusion.Blazor;

namespace ANPaX.UI.BlazorServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM3NTEzQDMxMzgyZTMzMmUzMGtiNmlyUGJxSktqUzdIVEd3TXpOK1hFekEyT2hDWStNa1lyK29RSUF6eWc9");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();
            services.AddSingleton(new ConnectionData
            {
                SQLConnectionString = "Default",
                SQLiteFilePath = Configuration.GetConnectionString("SQLiteFile")
            });

            services.AddSingleton<IDataAccess, SqliteDB>();
            services.AddSingleton<IParticleSimulationData, ParticleSimulationDataSQL>();
            services.AddSingleton<IAggregateConfigurationData, AggregateConfigurationDataSQLite>();
            services.AddSingleton<IFilmFormationConfigurationData, FilmFormationConfigurationDataSQL>();
            services.AddSingleton<IUserData, UserDataSQLite>();
            services.AddSingleton<IAggregateConfigurationModelService, AggregateConfigurationModelService>();
            services.AddSingleton<IUserModelService, UserModelService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
