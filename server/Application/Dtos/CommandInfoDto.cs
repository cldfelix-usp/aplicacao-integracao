namespace Server.Application.DTOs;

public class CommandInfoDto
{
    public string Operation { get; set; }
    public string Description { get; set; }
    public string Result { get; set; }
    public dynamic Format { get; set; }
    public CommandDto Command { get; set; }
}