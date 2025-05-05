using System.Net.Sockets;
using System.Text;

namespace Server.Models;

public class TelnetClient : IDisposable
{
    private TcpClient _client;
    private NetworkStream _stream;
    private readonly string _host;
    private readonly int _port;
    private readonly int _timeout;

    /// <summary>
    /// Inicializa um novo cliente Telnet
    /// </summary>
    /// <param name="host">Endereço IP ou hostname do dispositivo</param>
    /// <param name="port">Porta de comunicação</param>
    /// <param name="timeout">Timeout de conexão em milissegundos</param>
    public TelnetClient(string host, int port, int timeout = 5000)
    {
        _host = host;
        _port = port;
        _timeout = timeout;
    }

    /// <summary>
    /// Conecta ao dispositivo Telnet
    /// </summary>
    public async Task ConnectAsync()
    {
        _client = new TcpClient();

        try
        {
            await _client.ConnectAsync(_host, _port);
            _stream = _client.GetStream();
            _stream.ReadTimeout = _timeout;
            _stream.WriteTimeout = _timeout;
        }
        catch (Exception ex)
        {
            throw new Exception($"Falha ao conectar a {_host}:{_port}. Erro: {ex.Message}", ex);
        }
    }

    public async Task<string> ExecuteCommandAsync(CommandInfo selectedCommand)
        {
            if (_client == null || !_client.Connected)
            {
                throw new InvalidOperationException("Cliente não está conectado. Chame ConnectAsync() primeiro.");
            }

            // Constrói o comando conforme a especificação
            var commandBuilder = new StringBuilder();
            commandBuilder.Append(selectedCommand.Command.CommandName);

            // Adiciona os parâmetros separados por espaço
            foreach (var param in selectedCommand.Command.Parameters)
            {
                if (!string.IsNullOrEmpty(param.Name))
                {
                    commandBuilder.Append(' ');
                    commandBuilder.Append(param.Name);
                }
                // else if (param.IsRequired)
                // {
                //     throw new ArgumentException($"O parâmetro obrigatório '{param.Name}' não possui valor.");
                // }
            }

            // Adiciona o terminador de linha conforme especificado
            commandBuilder.Append('\r');

            var commandStr = commandBuilder.ToString();
            var commandBytes = Encoding.ASCII.GetBytes(commandStr);
            
            // Envia o comando
            await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);

            // Lê a resposta
            var response = await ReadResponseAsync();
            
            return response;
        }

        /// <summary>
        /// Lê a resposta do dispositivo até encontrar o terminador de linha '\r'
        /// </summary>
        private async Task<string> ReadResponseAsync()
        {
            var responseBuilder = new StringBuilder();
            var buffer = new byte[1024];
            int bytesRead;

            do
            {
                bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                var chunk = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                responseBuilder.Append(chunk);

                // Verifica se encontramos o terminador
                if (chunk.Contains('\r'))
                    break;

            } while (bytesRead > 0);

            return responseBuilder.ToString().TrimEnd('\r');
        }

        /// <summary>
        /// Fecha a conexão e libera recursos
        /// </summary>
        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
}