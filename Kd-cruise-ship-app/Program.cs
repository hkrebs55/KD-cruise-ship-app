namespace Kd_cruise_ship_app;

public class Program
{
    static void Main(string[] args)
    {
        SensorPOC sensorPOC = new SensorPOC();
        IngestorPOC ingestorPOC = new IngestorPOC();
        
        sensorPOC.WriteCSV();
        ingestorPOC.WriteLogFile();
        Console.ReadKey();
        sensorPOC.FinishWritingCSV();
        ingestorPOC.FinishWritingLogFile();
        
        
    }
}