﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoltOn.Data.EF;
using BoltOn.Samples.Application.Handlers;
using Microsoft.EntityFrameworkCore;
using BoltOn.Samples.Infrastructure.Data;
using BoltOn.Samples.Infrastructure.Data.Repositories;
using BoltOn.Data.CosmosDb;

namespace BoltOn.Samples.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.BoltOn(options =>
            {
                options.BoltOnEFModule();
                options.BoltOnCosmosDbModule();
                options.BoltOnAssemblies(typeof(PingHandler).Assembly, typeof(StudentRepository).Assembly);
            });

            services.AddDbContext<SchoolDbContext>(options =>
            {
                options.UseSqlServer("Data Source=127.0.0.1;initial catalog=Testing;persist security info=True;User ID=sa;Password=$Password1;");
            });

            //services.AddCosmosDbContext<CollegeDbContext>(new CosmosDbConfiguration
            //{
            //    Uri = "https://engagedb.documents.azure.com:443/",
            //    AuthorizationKey = "sswjWYMOGgfq8WpIxvTpSMfjeX05xJ6gQp971HsTdiIgQ0Dq9r0oXjABLtrQHj8CzYS60yXhZC8GvoFPUaSsuw==",
            //    DatabaseName = "College"
            //});

            services.AddCosmosDbContext<CollegeDbContext>(options =>
            {
                options.AuthorizationKey = string.Empty;
                options.Uri = string.Empty;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
            app.ApplicationServices.TightenBolts();
        }
    }
}
