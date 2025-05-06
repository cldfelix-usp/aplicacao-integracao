using Server.Application.DTOs;

namespace Server.Models;

public interface IDeviceManager
{
    void AddDevice(Device device);
    void LoadDevicesFromJson(string json);
    Device GetDeviceByName(string name);
    Task<List<CommandInfoDto>> GetCommandByName(string name);
    Task <List<DeviceDto>> ListDevices();
    Task<string> ExecuteCommandAsync(Device device, int commandIndex);
 
}