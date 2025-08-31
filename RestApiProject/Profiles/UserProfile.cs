using AutoMapper;
using RestApiProject.DTOs;
using RestApiProject.Models;

namespace RestApiProject.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Map from DTO to Model
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.FavoriteBookIds, opt => opt.Ignore()) // handled separately if needed
                .ForMember(dest => dest.WishlistBookIds, opt => opt.Ignore());

            // Map from Model to DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FavoriteBookIds, opt => opt.MapFrom(src => string.Join(",", src.FavoriteBookIds)))
                .ForMember(dest => dest.WishlistBookIds, opt => opt.MapFrom(src => string.Join(",", src.WishlistBookIds)));

            CreateMap<UserCreationDto, User>()
        .ForMember(dest => dest.FavoriteBookIds, opt => opt.Ignore()) // handled separately if needed
        .ForMember(dest => dest.WishlistBookIds, opt => opt.Ignore());

            // Map from Model to DTO
            CreateMap<User, UserCreationDto>()
                .ForMember(dest => dest.FavoriteBookIds, opt => opt.MapFrom(src => string.Join(",", src.FavoriteBookIds)))
                .ForMember(dest => dest.WishlistBookIds, opt => opt.MapFrom(src => string.Join(",", src.WishlistBookIds)));
            CreateMap<LoginRequestDto, User>();
            CreateMap<User, LoginRequestDto>();
        }
    }
}
