using AutoMapper;
using EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings.AutoMappers
{
    public class MaintenanceHistoryProfile : Profile
    {
        public MaintenanceHistoryProfile()
        {
            CreateMap<MaintenanceHistoryDungVm, MaintenanceHistoryDungVmDto>().ReverseMap();
            CreateMap<MaintenanceHistoryDungVm, MaintenanceHistoryDungVmCreateDto>().ReverseMap();
            CreateMap<MaintenanceHistoryDungVm, MaintenanceHistoryDungVmUpdateDto>().ReverseMap();
        }
    }
}
