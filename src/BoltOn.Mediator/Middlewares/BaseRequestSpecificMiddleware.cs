﻿using System;
using BoltOn.Mediator.Pipeline;

namespace BoltOn.Mediator.Middlewares
{
	public abstract class BaseRequestSpecificMiddleware<T> : IMediatorMiddleware
	{
		public MediatorResponse<TResponse> Run<TRequest, TResponse>(IRequest<TResponse> request,
																	  Func<IRequest<TResponse>, MediatorResponse<TResponse>> next)
			where TRequest : IRequest<TResponse>
		{
			if (!(request is T))
				return next.Invoke(request);
			return Execute<IRequest<TResponse>, TResponse>(request, next);
		}

		public abstract void Dispose();

		public abstract MediatorResponse<TResponse> Execute<TRequest, TResponse>(IRequest<TResponse> request,
																			   Func<IRequest<TResponse>, MediatorResponse<TResponse>> next);
	}
}