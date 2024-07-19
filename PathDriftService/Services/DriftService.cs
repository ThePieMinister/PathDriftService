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

      public override Task<PathDriftReply> GetPathDrift(PathDriftRequest request, IServerStreamWriter<PathDriftReply> responseStream, ServerCallContext context)
      {
         try
         {
            var reply = new PathDriftReply
            {
               Message = "One item here, please"
               //History[0] = new PathDriftItem
               //{
               //   ID = "Path_2",
               //   Index = 1,
               //   X = 10.0,
               //   Y = 11.0,
               //   Z = 12.0,
               //   Rx = 20.0,
               //   Ry = 21.0,
               //   Rz = 22.0
               //}
            };

            _logger.LogInformation($"gRPC Request received");
            return Task.FromResult(reply);
         }
         catch (Exception ex)
         {
            _logger.LogError("Error while handling your Request {Message}", ex.Message);
            throw;
         }
      }
   }
}
