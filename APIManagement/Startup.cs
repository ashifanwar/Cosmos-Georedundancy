using APIManagement.Azure.CosmosDb;
using APIManagement.Models;
using APIManagement.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using APIManagement.Contract;

namespace APIManagement
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c=> 
            {
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
            services.ConfigureSwaggerGen(options =>
            {
                SetSwaggerGenOptions(options);
            });
            
            services.AddScoped<IRepository<PersonalInfo>, PersonalInfoRespository>();
            services.AddConfiguration<CosmosDbPrimaryAccountSettings>(Configuration.GetSection("CosmosDbPrimaryAccountSettings"));
            services.AddConfiguration<CosmosDbSecondaryAccountSettings>(Configuration.GetSection("CosmosDbSecondaryAccountSettings"));

            services.AddOptions();

            services.Configure<CosmosDbPrimaryAccountSettings>(Configuration.GetSection("CosmosDbPrimaryAccountSettings"));
            services.Configure<CosmosDbSecondaryAccountSettings>(Configuration.GetSection("CosmosDbSecondaryAccountSettings"));

            var configDescriptor = new ServiceDescriptor(typeof(IConfigurationRoot), Configuration);
            services.Add(configDescriptor);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal API");
            });

        }

        private void SetSwaggerGenOptions(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Version = PersonalController.APIVERSION,
                Title = "API Management",
                Description = "Internal service providing resources related to API Management",
                TermsOfService = "None"
            });
        }

        private string GetXmlCommentsPath()
        {
            var app = System.AppContext.BaseDirectory;
            return System.IO.Path.Combine(app, "APIManagement.xml");
        }
    }
}
