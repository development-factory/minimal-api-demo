namespace MinimalApiDemo.Models;

public class Failure
{
    public int Sector { get; set; }
    public string Address { get; set; }
    public string FailureType { get; set; }
    public string Cause { get; set; }
    public DateTime Until { get; set; }
    public DateTime DateAndTime { get; set; }
}
