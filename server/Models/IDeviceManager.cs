using Server.Application.DTOs;

namespace Server.Models;

public interface IDeviceManager
{
    void AddDevice(Device device);
    void LoadDevicesFromJson(string json);
    Device GetDeviceByName(string name);
    List<CommandInfo> GetCommandByName(string name);
    List<DeviceDto> ListDevices();
    Task<string> ExecuteCommandAsync(Device device, int commandIndex);
 
}