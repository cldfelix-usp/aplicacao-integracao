using AutoMapper;
using Server.Application.DTOs;
using Server.Models;

namespace Server.Application.Map;

public class ServerMappingProfile: Profile
{
    public ServerMappingProfile()
    {
        CreateMap<Device, DeviceDto>();
        CreateMap<Command, CommandDto>();
        CreateMap<CommandInfo, CommandInfoDto>()
            .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format));
        CreateMap<Parameter ,ParameterDto>();
        // If you later need to uncomment the Commands property in DeviceDto,
        // you can add this mapping:
        // CreateMap<CommandInfo, CommandInfoDto>();
    }
}