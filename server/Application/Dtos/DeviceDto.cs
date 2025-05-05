namespace Server.Application.DTOs;

public class DeviceDto
{
    public string Id { get; set; }
    public string Identifier { get; set; }
    public string Description { get; set; }
    public string Manufacturer { get; set; }
    public string Url { get; set; }
    // public List<CommandInfoDto> Commands { get; set; } = [];
}