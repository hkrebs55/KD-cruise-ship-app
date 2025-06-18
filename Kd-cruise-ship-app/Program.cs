namespace Kd_cruise_ship_app;

public class Program
{
    static void Main(string[] args)
    {
        WriteCSVFile writeCSVFile = new WriteCSVFile();
        
        writeCSVFile.WriteCSV();
        Console.ReadKey();
        writeCSVFile.FinishWritingCSV();
        
        
    }
    
    
    
}