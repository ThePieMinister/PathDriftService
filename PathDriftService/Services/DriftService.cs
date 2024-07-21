using Grpc.Core;
using PathDriftService;

namespace PathDriftService.Services
{
   public class DriftService : PathDrift.PathDriftBase
   {
      private readonly ILogger<DriftService> _logger;
      public DriftService(ILogger<DriftService> logger)
      {
         _logger = logger;
      }

      public override async Task GetPathDrift(PathDriftRequest request, IServerStreamWriter<PathDriftItem> responseStream, ServerCallContext context)
      {
         try
         {
            _logger.LogInformation($"gRPC Request received");
            for (int i = 1; (i <= 5 && !context.CancellationToken.IsCancellationRequested); ++i)
            {
               var reply = new PathDriftItem
               {
                  ID = "Path_2",
                  Index = i,
                  X = 10.0f + i,
                  Y = 11.0f + i,
                  Z = 12.0f + i,
                  Rx = 20.0f,
                  Ry = 21.0f,
                  Rz = 22.0f
               };

               await responseStream.WriteAsync(reply);
               await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);
            }
         }
         catch (Exception ) when (context.CancellationToken.IsCancellationRequested)
         {
            _logger.LogInformation("Request was cancelled by the client");
         }
         catch (Exception ex)
         {
            _logger.LogError("Error while handling your Request {Message}", ex.Message);
            throw;
         }
      }

      public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
      {
         try
         {
            var reply = new HelloReply
            {
               Message = $"Hello, {request.Name}"
            };
            _logger.LogInformation($"GRPC Request Send and Recived");
            return Task.FromResult(reply);
         }
         catch (Exception ex)
         {
            _logger.LogError("Error while handing your request {Message}", ex.Message);
            throw;
         }
      }
   }
}
