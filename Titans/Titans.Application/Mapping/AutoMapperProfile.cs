using AutoMapper;
using Titans.Contract.Models.v1;

namespace Titans.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.User.User, User>();
            CreateMap<Domain.User.RefreshToken, RefreshToken>().ReverseMap();
        }
    }
}
