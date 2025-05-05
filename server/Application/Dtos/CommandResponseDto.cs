namespace Server.Application.DTOs;

public class CommandResponseDto
{
    public bool Success { get; set; }
    public string Response { get; set; }
    public string ErrorMessage { get; set; }
}