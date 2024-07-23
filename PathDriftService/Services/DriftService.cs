using Grpc.Core;
using PathDriftService;

namespace PathDriftService.Services
{

   public class DriftService(ILogger<DriftService> logger) : PathDrift.PathDriftBase
   {
      private readonly ILogger<DriftService> _logger = logger;

      public override async Task GetPathDrift(PathDriftRequest request, IServerStreamWriter<PathDriftItem> responseStream, ServerCallContext context)
      {
         try
         {
            //string filename = "..\\Data\\run1.csv";
            _logger.LogInformation($"gRPC Request received");
            // Todo Filename validation here
            string filename = request.Filename;
            _logger.LogInformation("GRPC Request for file {filename}", filename);

            // Get the datafile ready at the first useable line
            using (var sr = new CsvReader(filename, _logger))
            {
               await responseStream.WriteAsync(sr.Current());
               PathDriftItem? drift;
               while ((drift = sr.GetNext()) != null && !context.CancellationToken.IsCancellationRequested)
               {
                  await responseStream.WriteAsync(drift);
                  // Additional delay to check cancellation on shorter files
                  //await Task.Delay(TimeSpan.FromSeconds(0.2), context.CancellationToken);
               }
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
            _logger.LogInformation($"GRPC Request Sent and Received");
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
