namespace Kd_cruise_ship_app;

public class IngestorPOC
{
    private const string IngestorName = "IngestorPOC";
    private const string SensorName = "SensorPOC";
    private const int RecordIntervalMilliseconds = 120000; // 3 minutes
    private const string ReadFromDirectory = "C:\\Users\\hrkre\\Desktop\\Ks-cruise-ship-CSV-file"; //change to local folder
    private const string OutputDirectory = "C:\\Users\\hrkre\\Desktop\\Ks-cruise-ship-ingestor-file"; //change to local folder
 
    private int _currentFileNumSensor = 100;
    private int _currentFileNumIngestor = 100;
    private StreamWriter _currentFileWriter;
    private Timer _fileScanTimer;
    private readonly object _lockObject = new object();
    
    
    public string CurrentFileName { get; private set; }
    public string TargetFilePath { get; private set; }
    public string TargetFileName { get; private set; }
    public bool IsRunning { get; private set; }
    public int TotalLinesRead { get; set; }
    public int LinesWithNullVales { get; set; }
    
    
    public IngestorPOC()
    {
        EnsureDirectoriesExist();
    }
        
     
     private void LogMessage(string message)
     {
         string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
         Console.WriteLine(logEntry);
     }
     
     
     private void EnsureDirectoriesExist()
     {
         try
         {
             if (!Directory.Exists(ReadFromDirectory))
             {
                 LogMessage($"Read Directory Does Not Exists: {ReadFromDirectory}");
             }
             
             if (!Directory.Exists(OutputDirectory))
             {
                 Directory.CreateDirectory(OutputDirectory);;
             }
         }
         catch (Exception ex)
         {
             throw new Exception($"Failed to create directories: {ex.Message}", ex);
         }
     }
     
     
     private void CreateNewLogFile()
     {
         string currentDate = DateTime.Now.ToString("yyyyMMdd");
         string fileName = $"{IngestorName}_{currentDate}_{_currentFileNumIngestor}.txt";
         string filePath = Path.Combine(OutputDirectory, fileName);
         
         _currentFileWriter = new StreamWriter(filePath, false);
         CurrentFileName = fileName;
         
         LogMessage($"Created new Log file: {fileName}");
     }
     
     
     public void TimerCallBack(object state)
     {
         if (!IsRunning) return;
    
         lock (_lockObject)
         {
             try
             {
                 if (ScanForCurrentFile())
                 {
                     WriteRecordsToFile();
                     _currentFileNumSensor++;
                 }
                 LogMessage("CSV file Logged");
             }
             catch (Exception ex)
             {
                 //Throw could stop timer, Log instead to ensure time keeps going
                 LogMessage($"Error in timer callback: {ex.Message}");
             }
         }
     }
    
    
    private bool ScanForCurrentFile()
    {
            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                TargetFileName = $"{SensorName}_{currentDate}_{_currentFileNumSensor}.csv";
                string fullPath = Path.Combine(ReadFromDirectory, TargetFileName);
                
                TargetFilePath = fullPath;
                
                bool fileExists = File.Exists(fullPath);
    
                return fileExists;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to find CSV file: {ex.Message}", ex);
            }
    }
     
     private static SensorFileData ParseCsvLine(string line)
     {
         string[] parts = line.Split(',');
        
         if (parts.Length != 7)
         {
             throw new ArgumentException($"Expected 7 fields in CSV line, but got {parts.Length}: {line}");
         }
    
         var record = new SensorFileData
         {
             Timestamp = parts[0]?.Trim(),
             SensorID = parts[1]?.Trim(),
             SensorName = parts[2]?.Trim(),
             LatLabel = parts[3]?.Trim(),
             LatValue = parts[4]?.Trim(),
             LonLabel = parts[5]?.Trim(),
             LonValue = parts[6]?.Trim()
         };
         
         record.IsLatNull = string.IsNullOrEmpty(record.LatValue) || 
                            record.LatValue.Equals("null", StringComparison.OrdinalIgnoreCase);
         record.IsLonNull = string.IsNullOrEmpty(record.LonValue) || 
                            record.LonValue.Equals("null", StringComparison.OrdinalIgnoreCase);
    
         return record;
     }
     
     public (int TotalLinesRead, int LinesWithNullVales) ReadFileLines(string csvFilePath)
     { 
         List<SensorFileData> Records= new List<SensorFileData>();
        
         if (!File.Exists(csvFilePath))
         {
             throw new FileNotFoundException($"CSV file not found: {csvFilePath}");
         }
    
         string[] lines = File.ReadAllLines(csvFilePath);
         TotalLinesRead = lines.Length;
    
         foreach (string line in lines)
         {
             if (string.IsNullOrWhiteSpace(line))
                 continue;
    
             var record = ParseCsvLine(line);
             Records.Add(record);
             
             if (record.IsLatNull && record.IsLonNull)
                 LinesWithNullVales++;
         }
         
         return (TotalLinesRead, LinesWithNullVales);
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
                    
                 LogMessage($"Closed Log file: {CurrentFileName}");
                    
                 _currentFileNumIngestor++;
                 CurrentFileName = null;
             }
             catch (Exception ex)
             {
                 throw new Exception($"Error closing file: {ex.Message}", ex);
             }
         }
     }
     
     
     public void WriteRecordsToFile()
     {
    
         try
         {
             if (!File.Exists(TargetFilePath))
             {
                 LogMessage($"Target file does not exist: {TargetFilePath}");
                 return;
             }
             var (TotalLinesRead, LinesWithNullVales) = ReadFileLines(TargetFilePath);
    
             if (_currentFileWriter != null)
             {
                 _currentFileWriter.WriteLine($"TimeStamp: {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}");
                 _currentFileWriter.WriteLine($"Source: {SensorName}");
                 _currentFileWriter.WriteLine($"FileName: {TargetFileName}");
                 _currentFileWriter.WriteLine($"SuccessEntries: {TotalLinesRead}");
                 _currentFileWriter.WriteLine($"FailedEntries: {LinesWithNullVales}");
                 _currentFileWriter.WriteLine($"Errors: ");
                 _currentFileWriter.WriteLine($"");
                 
                 _currentFileWriter.Flush();
                 
             }
             else
             {
                 LogMessage("Error: File writer is not initialized");
             }
             
         }
         catch (Exception ex)
         {
             throw new Exception($"Error writing data entry: {ex.Message}", ex);
         }
     }
     
     
     public void WriteLogFile()
     {
         if (IsRunning)
         {
             LogMessage("Log processing is already running.");
             return;
         }
     
         try
         {
             IsRunning = true;
             LogMessage("Starting Log file processing...");
                 
             // Create initial file
             CreateNewLogFile();
             
             // Start timer for automatic file scanning
             _fileScanTimer = new Timer(TimerCallBack, null, 120000, RecordIntervalMilliseconds);
                 
             LogMessage("Log file processing started successfully.");
         }
         catch (Exception ex)
         {
             IsRunning = false;
             throw new Exception($"Failed to start Log processing: {ex.Message}", ex);
         }
     }
     
     
     public void FinishWritingLogFile()
     {
         if (!IsRunning)
         {
             LogMessage("Log processing is not running.");
             return;
         }
     
         try
         {
             IsRunning = false;
             
             //Stop timer
             _fileScanTimer = null;
             
             CloseCurrentFile();
             
             LogMessage("Log file processing stopped.");
         }
         catch (Exception ex)
         {
             throw new Exception($"Error stopping CSV processing: {ex.Message}", ex);
         }
     }
}