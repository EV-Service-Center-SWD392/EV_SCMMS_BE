using AutoMapper;
using EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm;
using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Infrastructure.Mappings.AutoMappers
{
    public class VehicleConditionProfile : Profile
    {
        public VehicleConditionProfile()
        {
            CreateMap<VehicleConditionDungVm, VehicleConditionDungVmDto>().ReverseMap();
            CreateMap<VehicleConditionDungVm, VehicleConditionDungVmCreateDto>().ReverseMap();
            CreateMap<VehicleConditionDungVm, VehicleConditionDungVmUpdateDto>().ReverseMap();
        }
        
    }
}
