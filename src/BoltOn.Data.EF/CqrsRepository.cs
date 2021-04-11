﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using BoltOn.Bus;
using BoltOn.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace BoltOn.Data.EF
{
	public class CqrsRepository<TEntity, TDbContext> : Repository<TEntity, TDbContext>
		where TDbContext : DbContext
		where TEntity : BaseCqrsEntity
	{
		private readonly IAppServiceBus _bus;
		private readonly IRepository<EventStore2> _eventStoreRepository;

		public CqrsRepository(TDbContext dbContext,
			IAppServiceBus bus,
			IRepository<EventStore2> repository) : base(dbContext)
		{
			_bus = bus;
			_eventStoreRepository = repository;
		}

		protected override async Task SaveChangesAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			await SaveChangesAsync(new[] { entity }, cancellationToken);
		}

		protected override async Task SaveChangesAsync(IEnumerable<TEntity> entities,
			CancellationToken cancellationToken = default)
		{
			var entitiesList = entities.ToList();
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
			{
				IsolationLevel = IsolationLevel.ReadCommitted
			}, TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var entity in entitiesList)
				{
					await AddEvents(entity, cancellationToken);
				}

				await DbContext.SaveChangesAsync(cancellationToken);
				transactionScope.Complete();
			}

			//entitiesList.ForEach(async e => await PublishEvents(e, cancellationToken));
		}

		protected async virtual Task AddEvents(TEntity entity, CancellationToken cancellationToken)
		{
			foreach (var e in entity.EventsToBeProcessed.ToList())
			{
				var eventStore = new EventStore2
				{
					EventId = e.Id,
					EntityId = entity.CqrsEntityId,
					EntityType = entity.GetType().FullName,
					CreatedDate = System.DateTimeOffset.Now,
					Data = e
				};

				await _eventStoreRepository.AddAsync(eventStore, cancellationToken);
			}
		}

		protected async virtual Task PublishEvents(TEntity entity, CancellationToken cancellationToken)
		{
			using var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
			{
				IsolationLevel = IsolationLevel.ReadCommitted
			}, TransactionScopeAsyncFlowOption.Enabled);
			var entityEvents = (await _eventStoreRepository.FindByAsync(f => f.EntityId == entity.CqrsEntityId &&
					f.EntityType == entity.GetType().FullName)).ToList();
			var eventsToBePublished = entityEvents.Select(s => s.Data).ToList();

			foreach (var @event in eventsToBePublished)
			{
				await _eventStoreRepository.DeleteAsync(@event, cancellationToken);
				await _bus.PublishAsync(@event, cancellationToken);
			}
			transactionScope.Complete();
		}
	}
}