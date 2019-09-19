﻿using System;
using BoltOn.Mediator.Pipeline;

namespace BoltOn.Cqrs
{
	public interface IEvent : IRequest
	{
		Guid Id { get; set; }
		string SourceTypeName { get; set; }
	}

	public class BoltOnEvent : IEvent
    {
        public Guid Id { get; set; }
        public string SourceTypeName { get; set; }
		public string SourceId { get; set; }
    }
}
