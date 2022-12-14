namespace Titans.SqlDb.Mapping;
using AutoMapper;
using Titans.SqlDb.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Domain.User.User, User>().ReverseMap();
        CreateMap<Domain.User.RefreshToken, RefreshToken>().ReverseMap();
    }
}