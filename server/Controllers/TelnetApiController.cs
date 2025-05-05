using Microsoft.AspNetCore.Mvc;
using Server.Application.DTOs;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelnetApiController : ControllerBase
{
    private readonly ITelnetApiService _telnetService;
    private readonly ILogger<TelnetApiController> _logger;
    private readonly IDeviceManager _deviceManager;

    public TelnetApiController(
        ITelnetApiService telnetService, 
        ILogger<TelnetApiController> logger, 
        IDeviceManager deviceManager)
    {
        _telnetService = telnetService;
        _logger = logger;
        _deviceManager = deviceManager;
    }

    /// <summary>
    /// Obtém a lista de dispositivos disponíveis
    /// </summary>
    [HttpGet("devices")]
    public ActionResult<List<DeviceDto>> GetDevices()
    {
        var devices = _deviceManager.ListDevices();
        return Ok(devices);
    }

    /// <summary>
    /// Obtém os comandos disponíveis para um dispositivo
    /// </summary>
    [HttpGet("devices/{deviceName}/commands")]
    public ActionResult<List<CommandDto>> GetDeviceCommands(
        string deviceName)
    {
        var commands = _deviceManager.GetCommandByName(deviceName);

       

        return Ok(commands);
    }

    /// <summary>
    /// Executa um comando em um dispositivo
    /// </summary>
    [HttpPost("devices/{deviceName}/commands/{commandIndex}/execute")]
    public async Task<ActionResult<CommandResponseDto>> ExecuteCommand(
        string deviceName,
        int commandIndex,
        [FromBody] ExecuteCommandDto executeCommand)
    {
        try
        {
            var response = await _telnetService.ExecuteCommandAsync(
                deviceName,
                commandIndex,
                executeCommand.ParameterValues);

            return Ok(new CommandResponseDto
            {
                Success = true,
                Response = response
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argumento inválido na execução do comando");
            return BadRequest(new CommandResponseDto
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar comando");
            return StatusCode(500, new CommandResponseDto
            {
                Success = false,
                ErrorMessage = "Erro interno ao executar o comando"
            });
        }
    }
}