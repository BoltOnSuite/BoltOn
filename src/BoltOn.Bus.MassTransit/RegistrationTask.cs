﻿using BoltOn.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;

namespace BoltOn.Bus.MassTransit
{
	public class RegistrationTask : IRegistrationTask
    {
        public void Run(RegistrationTaskContext context)
        {
            var serviceCollection = context.ServiceCollection;
            serviceCollection.AddSingleton<IBus, BoltOnMassTransitBus>();
			serviceCollection.AddTransient(typeof(BoltOnMassTransitConsumer<>));
        }
    }
}
