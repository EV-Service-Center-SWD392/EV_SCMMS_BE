using AutoMapper;
using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Domain.Models;


namespace EV_SCMMS.Infrastructure.Mappings.AutoMappers
{
    /// <summary>
    /// auto mapper sẽ tìm đến các thuộc tính cùng tên để map dữ liệu (ignore case)
    /// 
    /// cách tạo: CreateMap<Source, Destination>().ReverseMap();// ReverseMap nếu muốn map 2 chiều
    /// cách dùng: _mapper.Map<Destination>(source)
    /// 
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class MaintenanceTaskProfile : Profile
    {
        public MaintenanceTaskProfile()
        {
            CreateMap<MaintenanceTaskDungVm, MaintenanceTaskDungVmDto>().ReverseMap();
            CreateMap<MaintenanceTaskDungVm, MaintenanceTaskDungVmCreateDto>().ReverseMap();
            CreateMap<MaintenanceTaskDungVm, MaintenanceTaskDungVmUpdateDto>().ReverseMap();
        }
    }
}
        