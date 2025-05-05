namespace Server.Application.DTOs;

public class ResultBaseDto<T>
{
    public T? Data { get; private set; }
    public List<string> Errors { get; } = [];
    
    public ResultBaseDto(T data, List<string> errors)
    {
        Data = data;
        Errors = errors;
    }
    
    public ResultBaseDto(List<string> errors) => Errors = errors;
    
    public ResultBaseDto(T data) => Data = data;
    
    public ResultBaseDto(string error) => Errors.Add(error);
}