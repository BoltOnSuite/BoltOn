using System.Threading;
using System.Threading.Tasks;
using BoltOn.Requestor.Pipeline;

namespace BoltOn.Samples.Application.Handlers
{
	public class PingRequest : IRequest<PongResponse>
	{
	}

	public class PongResponse
	{
		public string Data { get; set; }
	}

	public class PingHandler : IHandler<PingRequest, PongResponse>
	{
		public async Task<PongResponse> HandleAsync(PingRequest request, CancellationToken cancellationToken)
		{
			return await Task.FromResult(new PongResponse { Data = "pong test 1 2" });
		}
	}
}
