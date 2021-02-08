using Microsoft.Extensions.DependencyInjection;
using BoltOn.Samples.Infrastructure.Data;
using System;
using BoltOn.Bootstrapping;
using BoltOn.Samples.Application.Entities;

namespace BoltOn.Samples.WebApi
{
    public class PostRegistrationTask // : IPostRegistrationTask
    {
        private readonly IServiceProvider _serviceProvider;

        public PostRegistrationTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Run()
        {
            using var scope = _serviceProvider.CreateScope();
            var writeDbContext = scope.ServiceProvider.GetService<SchoolDbContext>();
			//writeDbContext.Database.EnsureDeleted();
			writeDbContext.Database.EnsureCreated();

            var inState = new StudentType(1, "In-State");
            var outOfState = new StudentType(2, "Out-of-State");
            writeDbContext.Set<StudentType>().Add(inState);
            writeDbContext.Set<StudentType>().Add(outOfState);
            writeDbContext.SaveChanges();
		}
    }
}
