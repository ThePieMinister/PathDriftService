using PathDriftService.Services;

namespace PathDriftService
{
   internal class CsvReader : IDisposable
   {
      readonly ILogger<DriftService>? _logger;
      readonly StreamReader? _sr;
      string? _line;

      private CsvReader() => _sr = null;
      internal CsvReader(string filename, ILogger<DriftService> logger)
      {
         _sr = new StreamReader(filename);
         _logger = logger;
         if (_sr == null)
         {
            _logger.LogError("StreamReader could not be created");
            return;
         }

         while ((_line = _sr.ReadLine()) != null)
         {
            _line = _line.Trim();

            if (string.IsNullOrEmpty(_line))
               continue;
            var values = _line.Split(',', StringSplitOptions.TrimEntries);

            if (values[1] != "Index")
               break;
         }
      }

      internal PathDriftItem? Current() => Parse(_line);

      internal static PathDriftItem? Parse(string line)
      {
         var values = line.Split(',', StringSplitOptions.TrimEntries);
         var drift = new PathDriftItem
         {
            ID = values[0],
            Index = Int32.Parse(values[1]),
            X = float.Parse(values[2]),
            Y = float.Parse(values[3]),
            Z = float.Parse(values[4]),
            Rx = float.Parse(values[5]),
            Ry = float.Parse(values[6]),
            Rz = float.Parse(values[7])
         };
         return drift;
      }

      internal PathDriftItem? GetNext()
      {
         PathDriftItem? drift = null;
         if (!_sr.EndOfStream)
         {
            _line = _sr.ReadLine();
            try
            {
               _line = _line.Trim();
               // Perhaps need to cope with empty or mal-formed lines

               drift = Parse(_line);
            }
            catch (Exception ex)
            {
               _logger.LogError("Error reading datafile: {Message}", ex.Message);
            }
         }
         return drift;
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            _sr?.Dispose();
         }
      }
   }
}
