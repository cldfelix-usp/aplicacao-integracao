using System.Text.Json;
using AutoMapper;
using Server.Application.DTOs;

namespace Server.Models;

public class DeviceManager : IDeviceManager
    {
        private readonly List<Device> _devices = [];
        private readonly IMapper _mapper;

        public DeviceManager(IMapper mapper)
        {
            _mapper = mapper;
            // Caminho do arquivo de configuração de dispositivos
            var devicesConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "devices.json");
            var json = File.ReadAllText(devicesConfigPath);
            LoadDevicesFromJson(json);
        }

        /// <summary>
        /// Adiciona um dispositivo à lista de dispositivos gerenciados
        /// </summary>
        public void AddDevice(Device device)
        {
            _devices.Add(device);
        }

        /// <summary>
        /// Carrega dispositivos a partir de um arquivo JSON
        /// </summary>
        public void LoadDevicesFromJson(string json)
        {
            try
            {
                var devices = JsonSerializer.Deserialize<List<Device>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _devices.AddRange(devices);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Erro ao carregar dispositivos do JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtém um dispositivo pelo nome
        /// </summary>
        public Device GetDeviceByName(string name)
        {
            var device  = _devices.Find(d => d.Id.Equals(name, StringComparison.OrdinalIgnoreCase));
            var comands  = _mapper.Map<List<CommandDto>>(device.Commands);
            return device;
        }
        
        public async Task<List<CommandInfoDto>> GetCommandByName(string name)
        {
            var device  = _devices.Find(d => d.Id.Equals(name, StringComparison.OrdinalIgnoreCase));
            var comands  = _mapper.Map<List<CommandInfoDto>>(device.Commands);

            return await Task.FromResult(comands);
        }

        /// <summary>
        /// Lista todos os dispositivos disponíveis
        /// </summary>
        public async Task<List<DeviceDto>> ListDevices()
        { 
            return await  Task.FromResult( _mapper.Map< List<DeviceDto>>(_devices));
        }

        /// <summary>
        /// Executa um comando em um dispositivo
        /// </summary>
        /// <param name="device">O dispositivo alvo</param>
        /// <param name="commandIndex">Índice do comando a ser executado</param>
        /// <returns>Resposta formatada do dispositivo</returns>
        public async Task<string> ExecuteCommandAsync(Device device, int commandIndex)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if (commandIndex < 0 || commandIndex >= device.Commands.Count)
                throw new ArgumentOutOfRangeException(nameof(commandIndex), "Índice de comando inválido.");

            var commandInfo = device.Commands[commandIndex];
            var url = device.Url.Split(":")[0];
            var port = int.Parse(device.Url.Split(":")[1]);

            using var client = new TelnetClient(url, port);
            await client.ConnectAsync();
            var rawResponse = await client.ExecuteCommandAsync(commandInfo);
                
            // Aqui você poderia implementar a formatação da resposta de acordo com
            // o formato especificado em commandInfo.Format, se necessário
                
            return FormatResponse(rawResponse, commandInfo.Format);
        }

        /// <summary>
        /// Formata a resposta de acordo com o formato especificado
        /// </summary>
        private string FormatResponse(string rawResponse, string format)
        {
            // Esta é uma implementação simplificada.
            // Você pode expandir isso para interpretar e formatar a resposta
            // de acordo com o formato especificado no comando

            if (string.IsNullOrEmpty(format))
                return rawResponse;

            // Aqui você implementaria a lógica para formatar a resposta
            // com base no formato definido no comando
            
            // Por enquanto, apenas retornamos a resposta crua
            return rawResponse;
        }
    }