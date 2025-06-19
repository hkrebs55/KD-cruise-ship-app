namespace Kd_cruise_ship_app;

public class SensorFileData
{
    public string Timestamp { get; set; }
    public string SensorID { get; set; }
    public string SensorName { get; set; }
    public string LatLabel { get; set; }
    public string LatValue { get; set; }
    public string LonLabel { get; set; }
    public string LonValue { get; set; }
    public bool IsLatNull { get; set; }
    public bool IsLonNull { get; set; }
}