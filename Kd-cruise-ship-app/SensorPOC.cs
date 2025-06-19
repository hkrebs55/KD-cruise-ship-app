namespace Kd_cruise_ship_app;

public class SensorPOC
{
    
    private const string SensorName = "SensorPOC";
    private const int SensorID = 100;
    private const int MaxEntriesFileAllotment = 100;
    private const int RecordIntervalMillisecond = 1000; // 1 second
    private const string OutputDirectory = "C:\\Users\\hrkre\\Desktop\\Ks-cruise-ship-CSV-file"; //change to local folder
    
    
    private int _currentFileNum = 100;
    private int _currentEntriesCount = 0;
    private StreamWriter _currentFileWriter;
    private Timer _recoredTimer;
    private Random _random;
    private readonly object _lockObject = new object();
    
    
    public bool IsRunning { get; private set; }
    public string CurrentFileName { get; private set; }
    
    
    public SensorPOC()
    {
        _random = new Random();
        EnsureOutputDirectoryExists();
    }
        
    
    private void LogMessage(string message)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            
        // Also write to console for immediate feedback
        Console.WriteLine(logEntry);
    }
    
    
    private void EnsureOutputDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create output directory: {ex.Message}", ex);
        }
    }
    
    private void CreateNewCSVFile()
    {
        try
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = $"{SensorName}_{currentDate}_{_currentFileNum}.csv";
            string filePath = Path.Combine(OutputDirectory, fileName);
                
            _currentFileWriter = new StreamWriter(filePath, false);
            CurrentFileName = fileName;
            _currentEntriesCount = 0;
                
            LogMessage($"Created new CSV file: {fileName}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create new CSV file: {ex.Message}", ex);
        }
    }
    
    private string FormatCSVLine(double? latValue, double? lonValue)
    {
        string timestampStr = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        string latStr = Convert.ToString(latValue);
        string lonStr = Convert.ToString(lonValue);
            
        return $"{timestampStr},{SensorID},{SensorName},Lat,{latStr},Lon,{lonStr}";
    }
    
    private (double? lat, double? lon) GenerateRandomCoordinates()
    {
        // 10% chance of generating null values
        if (_random.NextDouble() < 0.1)
        {
            return (null, null);
        }

        double latValue = (_random.NextDouble() * (90.0 - (-90.0))) + (-90.0);
        double lonValue = (_random.NextDouble() * (180.0 - (-180.0))) + (-180.0);
        
        latValue = Math.Round(latValue, 2);
        lonValue = Math.Round(lonValue, 2);
            
        return (latValue, lonValue);
    }
    
    
    private void CloseCurrentFile()
    {
        if (_currentFileWriter != null)
        {
            try
            {
                _currentFileWriter.Flush();
                _currentFileWriter.Close();
                _currentFileWriter.Dispose();
                _currentFileWriter = null;
                    
                LogMessage($"Closed CSV file: {CurrentFileName} with {_currentEntriesCount} entries");
                    
                _currentFileNum++;
                CurrentFileName = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error closing file: {ex.Message}", ex);
            }
        }
    }
    
    public void WriteDataEntry()
    {
        lock (_lockObject)
        {
            try
            {
                // Create new file if needed
                if (_currentFileWriter == null)
                {
                    CreateNewCSVFile();
                }
                    
                DateTime timestamp = DateTime.UtcNow;
                var (latValue, lonValue) = GenerateRandomCoordinates();
            
                string csvLine = FormatCSVLine(latValue, lonValue);
            
                _currentFileWriter.WriteLine(csvLine);
                _currentFileWriter.Flush();
                _currentEntriesCount++;
                    
                // Check if file is full
                if (_currentEntriesCount >= MaxEntriesFileAllotment)
                {
                    CloseCurrentFile();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing data entry: {ex.Message}", ex);
            }
        }
    }
    
    private void TimerCallback(object state)
    {
        if (!IsRunning) return;
            
        lock (_lockObject)
        {
            try
            {
                WriteDataEntry();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in timer callback: {ex.Message}", ex);
            }
        }
    }
    
    
    public void WriteCSV()
    {
        if (IsRunning)
        {
            LogMessage($"Process already running.");
            return;
        }

        try
        {
            IsRunning = true;
            LogMessage("Starting CSV writing process...");
                
            // Create initial file
            CreateNewCSVFile();
                
            // Start timer for automatic data generation
            _recoredTimer = new Timer(TimerCallback, null, 0, RecordIntervalMillisecond);
                
            LogMessage("CSV writing process started successfully.");
        }
        catch (Exception ex)
        {
            IsRunning = false;
            throw new Exception($"Failed to start CSV writing: {ex.Message}", ex);
        }
    }
    
    
    public void FinishWritingCSV()
    {
        if (!IsRunning) return;
    
        try
        {
            IsRunning = false;
                
            // Stop timer
            _recoredTimer = null;
            
            CloseCurrentFile();
                
            LogMessage("CSV writing process stopped.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error stopping CSV writing: {ex.Message}", ex);
        }
    }
}
