namespace Server.Application.DTOs;

public class CommandDto
{
    public string CommandName { get; set; }
    public List<ParameterDto> Parameters { get; set; } = [];
}