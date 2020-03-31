using System.Threading;
using System.Threading.Tasks;
using BoltOn.Requestor.Pipeline;

namespace BoltOn.Bus
{
	public interface IBus
	{
		Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
			where TMessage : IRequest;
	}
}
