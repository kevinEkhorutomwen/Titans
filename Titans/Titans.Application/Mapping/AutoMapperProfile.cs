namespace Titans.Application.Mapping;
using AutoMapper;
using Titans.Contract.Models.v1;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Domain.User.User, User>();
        CreateMap<Domain.User.RefreshToken, RefreshToken>().ReverseMap();
    }
}