﻿using System;
using System.Linq;
using BoltOn.Bootstrapping;
using BoltOn.Cqrs;
using BoltOn.Requestor.Interceptors;
using BoltOn.Tests.Bootstrapping.Fakes;
using BoltOn.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoltOn.Tests.Bootstrapping
{
	[Collection("IntegrationTests")]
    public class BootstrapperTests : IDisposable
    {
		public BootstrapperTests()
		{
			BootstrapperRegistrationTasksHelper.Tasks.Clear();
		}

        [Fact]
        public void BoltOn_DefaultBoltOnWithAllTheAssemblies_RunsRegistrationTasksAndResolvesDependencies()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
            serviceCollection.AddTransient<Employee>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // act 
            var employee = serviceProvider.GetRequiredService<Employee>();

            // assert
            var name = employee.GetName();
            Assert.Equal("John", name);
        }

        [Fact]
        public void BoltOn_DefaultBoltOnWithAllTheAssemblies_ResolvesDependenciesRegisteredByConvention()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // act 
            var result = serviceProvider.GetRequiredService<ITestService>();

            // assert
            var name = result.GetName();
            Assert.Equal("test", name);
        }

        [Fact]
        public void BoltOn_BoltOnWithoutTightenBolts_DoesNotExecutePostRegistrationTask()
        {
            // arrange
            var serviceCollection = new ServiceCollection();

            // act 
            serviceCollection.BoltOn();

            // assert
            var postRegistrationTaskIndex = BootstrapperRegistrationTasksHelper.Tasks.IndexOf($"Executed {typeof(TestBootstrapperPostRegistrationTask).Name}");
            Assert.True(postRegistrationTaskIndex == -1);
        }

        [Fact]
        public void BoltOn_BoltOnAndTightenBolts_ExecutesAllPostRegistrationTasksInOrder()
        {
            // arrange
            BootstrapperRegistrationTasksHelper.Tasks.Clear();
            var serviceCollection = new ServiceCollection();

            // act 
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.TightenBolts();

            // assert
            var postRegistrationTaskIndex = BootstrapperRegistrationTasksHelper.Tasks.IndexOf($"Executed {typeof(TestBootstrapperPostRegistrationTask).Name}");
            Assert.True(postRegistrationTaskIndex != -1);
        }

        [Fact]
        public void BoltOn_BoltOnAndTightenBoltsWithExcludedFromRegistrationClass_ReturnsNull()
        {
            // arrange
            var serviceCollection = new ServiceCollection();

            // act 
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.TightenBolts();
            var test = serviceProvider.GetService<ITestExcludeRegistrationService>();

            // assert
            Assert.Null(test);
        }

        [Fact]
        public void BoltOn_BoltOnAndTightenBoltsWithExcludedFromRegistrationInterface_ReturnsNull()
        {
            // arrange
            var serviceCollection = new ServiceCollection();

            // act 
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.TightenBolts();
            var test = serviceProvider.GetService<ITestExcludeRegistrationService2>();

            // assert
            Assert.Null(test);
        }

        [Fact]
        public void BoltOn_TightenBoltsCalledMoreThanOnce_PostRegistrationTasksGetCalledOnce()
        {
			// arrange
			var serviceCollection = new ServiceCollection();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // act
            serviceProvider.TightenBolts();
            serviceProvider.TightenBolts();

            // assert
            var postRegistrationTaskCount = BootstrapperRegistrationTasksHelper.Tasks
                                    .Count(w => w == $"Executed {typeof(TestBootstrapperPostRegistrationTask).Name}");
            Assert.True(postRegistrationTaskCount == 1);
        }

        [Fact]
        public void BoltOn_TightenBolts_PostRegistrationWithDependencyGetCalledOnce()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // act
            serviceProvider.TightenBolts();

            // assert
            var postRegistrationTaskCount = BootstrapperRegistrationTasksHelper.Tasks
                                    .Count(w => w == "Executed test service");
            Assert.True(postRegistrationTaskCount == 1);
        }

		[Fact]
		public void GetService_WithAllDefaultInterceptors_ReturnsRegisteredInterceptorsInOrder()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.BoltOn();
			var serviceProvider = serviceCollection.BuildServiceProvider();
			serviceProvider.TightenBolts();

			// act
			var interceptors = serviceProvider.GetServices<IInterceptor>().ToList();

			// assert
			Assert.NotNull(interceptors.FirstOrDefault(f => f.GetType() == typeof(StopwatchInterceptor)));
			Assert.NotNull(interceptors.FirstOrDefault(f => f.GetType() == typeof(TransactionInterceptor)));
		}

        [Fact]
        public void GetService_BootstrapperOptionsWithDefaultInterceptors_ReturnsRegisteredInterceptorsInOrder()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.BoltOn();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.TightenBolts();

            // act
            var bootstrapperOptions = serviceProvider.GetService<BootstrapperOptions>();

            // assert
            var interceptorTypes = bootstrapperOptions.InterceptorTypes.ToList();
            var stopWatchInterceptorIndex = interceptorTypes.IndexOf(typeof(StopwatchInterceptor));
            var transactionInterceptorIndex = interceptorTypes.IndexOf(typeof(TransactionInterceptor));
			Assert.True(stopWatchInterceptorIndex != -1);
            Assert.True(transactionInterceptorIndex != -1);
			Assert.True(stopWatchInterceptorIndex < transactionInterceptorIndex);
        }

		public void Dispose()
        {
        }
    }
}
