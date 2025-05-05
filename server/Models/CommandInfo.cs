namespace Server.Models;

public class CommandInfo
{
    public string Operation { get; set; }
    public string Description { get; set; }
    public string Result { get; set; }
    public dynamic Format { get; set; }
    public Command Command { get; set; }
}