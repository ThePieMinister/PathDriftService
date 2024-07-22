using PathDriftService;
using Grpc.Net.Client;
using Grpc.Core;

var channel = GrpcChannel.ForAddress("https://localhost:7262");
var client = new PathDrift.PathDriftClient(channel);
Console.WriteLine("Please enter the name of a PathDriftDate CSV file to read:");
string filename = Console.ReadLine() ?? "Missing filename";

//var response = await client.SayHelloAsync(new HelloRequest { Name = filename });
//Console.WriteLine(response);
//Console.ReadLine();


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

using var call = client.GetPathDrift(new PathDriftRequest { Filename = filename }, null, null, cts.Token);
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