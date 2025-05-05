using Server.Models;

namespace Server.Services;

public class TelnetApiService : ITelnetApiService
    {
        private readonly IDeviceManager _deviceManager;
        private readonly ILogger<TelnetApiService> _logger;

        public TelnetApiService(IDeviceManager deviceManager, ILogger<TelnetApiService> logger)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa um comando em um dispositivo especificado pelo nome
        /// </summary>
        /// <param name="deviceName">Nome do dispositivo</param>
        /// <param name="commandIndex">Índice do comando</param>
        /// <param name="parameterValues">Valores dos parâmetros</param>
        /// <returns>Resposta do comando</returns>
        /// <exception cref="ArgumentException">Se o dispositivo não for encontrado</exception>
        public async Task<string> ExecuteCommandAsync(string deviceName, int commandIndex, string[] parameterValues)
        {
            _logger.LogInformation($"Executando comando {commandIndex} no dispositivo {deviceName}");
            
            var device = _deviceManager.GetDeviceByName(deviceName);
            if (device == null)
            {
                _logger.LogError($"Dispositivo não encontrado: {deviceName}");
                throw new ArgumentException($"Dispositivo não encontrado: {deviceName}");
            }

            if (commandIndex < 0 || commandIndex >= device.Commands.Count)
            {
                _logger.LogError($"Índice de comando inválido: {commandIndex}");
                throw new ArgumentOutOfRangeException(nameof(commandIndex), "Índice de comando inválido");
            }

            // Definir os valores dos parâmetros
            var command = device.Commands[commandIndex].Command;
            
            // Verificar se o número de parâmetros corresponde
            if (parameterValues.Length > command.Parameters.Count)
            {
                _logger.LogWarning($"Número de parâmetros fornecidos ({parameterValues.Length}) é maior que o esperado ({command.Parameters.Count})");
            }

            // Atribuir valores aos parâmetros
            for (var i = 0; i < Math.Min(parameterValues.Length, command.Parameters.Count); i++)
            {
                command.Parameters[i].Name = parameterValues[i];
            }

            // Verificar parâmetros obrigatórios
            for (var i = 0; i < command.Parameters.Count; i++)
            {
                var param = command.Parameters[i];
                if ((i >= parameterValues.Length || string.IsNullOrWhiteSpace(param.Name)))
                {
                    _logger.LogError($"Parâmetro obrigatório não fornecido: {param.Name}");
                    throw new ArgumentException($"O parâmetro obrigatório '{param.Name}' não foi fornecido.");
                }
            }

            try
            {
                var response = await _deviceManager.ExecuteCommandAsync(device, commandIndex);
                _logger.LogInformation($"Comando executado com sucesso: {response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao executar comando: {ex.Message}");
                throw;
            }
        }
    }