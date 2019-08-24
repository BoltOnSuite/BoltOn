using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BoltOn.Bus.RabbitMq;
using BoltOn.Logging;
using Moq;
using BoltOn.Tests.Other;
using System.Linq;
using BoltOn.Bootstrapping;
using MassTransit;
using static BoltOn.Bus.RabbitMq.Extensions;

namespace BoltOn.Tests.Bus
{
	[Collection("IntegrationTests")]
	public class MassTransitBoltOnBusTests : IDisposable
	{
		public MassTransitBoltOnBusTests()
		{
			Bootstrapper
				.Instance
				.Dispose();
		}

		[Fact]
		public async Task Publish_Message_GetsConsumed()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.BoltOn(b =>
			{
				b.BoltOnAssemblies(GetType().Assembly);
				b.BoltOnRabbitMqBusModule();
			});

			serviceCollection.AddMassTransit(x =>
			{
				x.AddBus(provider => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
				{
					cfg.ReceiveEndpoint("CreateTestStudent_queue", ep =>
					{
						ep.Consumer<MassTransitRequestConsumer<CreateTestStudent>>();
					});
				}));
			});


			var logger = new Mock<IBoltOnLogger<CreateTestStudentHandler>>();
			logger.Setup(s => s.Debug(It.IsAny<string>()))
								.Callback<string>(st => MediatorTestHelper.LoggerStatements.Add(st));
			serviceCollection.AddTransient((s) => logger.Object);

			var serviceProvider = serviceCollection.BuildServiceProvider();
			serviceProvider.TightenBolts();
			var bus = serviceProvider.GetService<BoltOn.Bus.IBus>();

			// act
			await bus.PublishAsync(new CreateTestStudent { FirstName = "test" });
			// as assert not working after async method, added sleep
			Thread.Sleep(1000);

			// assert
			var result = MediatorTestHelper.LoggerStatements.FirstOrDefault(f => f ==
										$"{nameof(CreateTestStudentHandler)} invoked");
			Assert.NotNull(result);
		}

		public void Dispose()
		{
			MediatorTestHelper.LoggerStatements.Clear();
			Bootstrapper
				.Instance
				.Dispose();
		}
	}
}
