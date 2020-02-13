﻿using System;
using Microsoft.Extensions.DependencyInjection;
using BoltOn.Bootstrapping;
using BoltOn.Tests.Other;

namespace BoltOn.Tests.Data.EF
{
	public class PostRegistrationTask : IPostRegistrationTask
	{
        private readonly IServiceProvider _serviceProvider;

        public PostRegistrationTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

		public void Run()
		{
			if (IntegrationTestHelper.IsSeedData)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var testDbContext = scope.ServiceProvider.GetService<SchoolDbContext>();
                    if (testDbContext != null)
                    {
                        testDbContext.Database.EnsureDeleted();
                        testDbContext.Database.EnsureCreated();

                        testDbContext.Set<Student>().Add(new Student
                        {
                            Id = 1,
                            FirstName = "a",
                            LastName = "b"
                        });
                        var student = new Student
                        {
                            Id = 2,
                            FirstName = "x",
                            LastName = "y"
                        };
                        testDbContext.Set<Student>().Add(new Student
                        {
                            Id = 10,
                            FirstName = "record to be deleted",
                            LastName = "b"
                        });
                        testDbContext.Set<Student>().Add(new Student
                        {
                            Id = 11,
                            FirstName = "record to be deleted",
                            LastName = "b"
                        });
                        var address = new Address {Id = Guid.NewGuid(), Street = "Computer Science", Student = student};
                        testDbContext.Set<Student>().Add(student);
                        testDbContext.Set<Address>().Add(address);
                        testDbContext.SaveChanges();
                        testDbContext.Dispose();
                    }
                }
			}
		}
	}
}
