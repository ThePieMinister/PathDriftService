using PathDriftService;
using Grpc.Net.Client;
using Grpc.Core;

var channel = GrpcChannel.ForAddress("https://localhost:7262");
var client = new PathDrift.PathDriftClient(channel);
Console.WriteLine("Please Enter a Path ID:");
string pathId = Console.ReadLine() ?? "Missing Path ID";

var response = await client.SayHelloAsync(new HelloRequest { Name = pathId });
Console.WriteLine(response);
Console.ReadLine();


using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) =>
{
   // We'll stop the process manually by using the CancellationToken
   e.Cancel = true;

   // Change the state of the CancellationToken to "Canceled"
   // - Set the IsCancellationRequested property to true
   // - Call the registered callbacks
   cts.Cancel();
};

using var call = client.GetPathDrift(new PathDriftRequest { ID = pathId }, null, null, cts.Token);
try
{
   await foreach (var pathItem in call.ResponseStream.ReadAllAsync())
   {
      Console.Write($"ID: {pathItem.ID}, Index: {pathItem.Index}, ");
      Console.Write($"X: {pathItem.X}, Y: {pathItem.Y}, Z: {pathItem.Z}, ");
      Console.WriteLine($"Rx: {pathItem.Rx}, Ry: {pathItem.Ry}, Rz: {pathItem.Rz}");
   }
}
catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Cancelled)
{
   Console.WriteLine(ex.Status.Detail);
}
catch (RpcException ex)
{
   Console.WriteLine(ex.Message);
}

Console.ReadLine();