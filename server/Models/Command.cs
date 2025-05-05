using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Server.Models;

public class Command
{
    [JsonPropertyName("command")]
    public string CommandName { get; set; }
    
    [JsonPropertyName("parameters")]
    public List<Parameter> Parameters { get; set; } = [];
}