using AutoMapper;
using POS.Application.Commons.Select.Response;
using POS.Application.Dtos.User.Request;
using POS.Application.Dtos.User.Response;
using POS.Domain.Entities;
using POS.Utilities.Static;

namespace POS.Application.Mappers
{
    public class UserMappingsProfile : Profile
    {
        public UserMappingsProfile()
        {
            CreateMap<User, UserResponseDto>()
                .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.StateUser,
                    x => x.MapFrom(y => y.State.Equals((int)StateTypes.Active) ? "Activo" : "Inactivo"))
                .ReverseMap();

            CreateMap<User, SelectResponse>()
                .ForMember(x => x.Description, x => x.MapFrom(y => y.UserName))
                .ReverseMap();

            CreateMap<User, UserByIdResponseDto>()
                .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id))
                .ReverseMap();

            CreateMap<UserRequestDto, User>();
            CreateMap<TokenRequestDto, User>();
        }
    }
}