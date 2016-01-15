using ClientDirectory.Helpers;
using ClientDirectory.Interfaces;
using ClientDirectory.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ClientDirectory
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        // This method gets called by the runtime. This is used to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();

            app.UseCors("AllowAll");
        }

        // This method gets called by the runtime. It is used to add services to the container (DI).
        public void ConfigureServices(IServiceCollection services)
        {
            // Helpers
            services.AddScoped<ISecurityHelper, SecurityHelper>();
            services.AddScoped<ILoginHelper, LoginHelper>();            

            // Router configuration
            services.ConfigureRouting(routeOptions =>
            {
                routeOptions.AppendTrailingSlash = true;
                routeOptions.LowercaseUrls = true;
            });

            // CORS
            services.AddCors(options =>
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

            // Add and configure MVC
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.Formatting = Formatting.Indented;
            });

            // Add and configure EntityFramework
            var connection = Configuration.GetSection("Data:ClientDirectoryDb").Value;

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ClientDirectoryContext>(options => options.UseSqlServer(connection));
        }
    }
}