using AutoMapper;
using EV_SCMMS.Core.Domain;
namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ApplicationUser mappings
        // CreateMap<ApplicationUser, UserInfoDto>()
        //     .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
        //     .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles are loaded separately

        // // User entity mappings
        // CreateMap<User, UserDto>().ReverseMap();

        // // Register DTO mappings
        // CreateMap<RegisterDto, ApplicationUser>()
        //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
        //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
        //     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        //     .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
        //     .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        //     .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

    }
}
