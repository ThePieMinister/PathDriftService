# PathDriftService
This Repository contains a Microsoft Visual Studio solution file which consisting of two projects written for ASP.NET 8. There is also a Console project which is not part of the solution which is a gRPC client. 

There is a common PathDrift.proto file in the repository in ./Protos, which is used by each of the projects.

In theory, the projects should be useable from VS Code in Linux as well as Windows, though this has not been tested.

## PathDriftService
This project is designed to read a CSV file containing path drift data for a series of measurements, and then make them available via a gRPC service to any application that is able to read that service. The file to be read is passed in the request message, and then the data is returned as one streaming reply (i.e. Server Streaming mode).

There is no security currently in place.

## PathDriftBlazorClient
The second project is a Blazor web application, not WebAssembly, which requests the data from the file run1.csv in the ./TestData folder when requested, and displays it in the home page as a list. I had hioped to be able to add graphical views of the path drift, but I do not know enough about available Blazor graphing components to do this at present.

## PathDriftClient
This is a third project which was developed to prove that the server service was working and able to provide data. It only displays retrieved data in the console.
