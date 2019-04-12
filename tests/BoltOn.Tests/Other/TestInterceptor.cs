﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BoltOn.Logging;
using BoltOn.Mediator.Interceptors;
using BoltOn.Mediator.Pipeline;

namespace BoltOn.Tests.Other
{
	public class TestInterceptor : IInterceptor
	{
		private readonly IBoltOnLogger<TestInterceptor> _logger;

		public TestInterceptor(IBoltOnLogger<TestInterceptor> logger)
		{
			_logger = logger;
		}

		public TResponse Run<TRequest, TResponse>(IRequest<TResponse> request,
																	 Func<IRequest<TResponse>, TResponse> next)
		   where TRequest : IRequest<TResponse>
		{
			_logger.Debug("TestInterceptor Started");
			var response = next.Invoke(request);
			_logger.Debug("TestInterceptor Ended");
			return response;
		}

		public void Dispose()
		{
		}

		public async Task<TResponse> RunAsync<TRequest, TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken, 
			Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> next) where TRequest : IRequest<TResponse>
		{
			_logger.Debug("TestInterceptor Started");
			var response = await next.Invoke(request, cancellationToken);
			_logger.Debug("TestInterceptor Ended");
			return response;
		}
	}
}
