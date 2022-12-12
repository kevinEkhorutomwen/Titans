using AutoMapper;
using Titans.SqlDb.Models;

namespace Titans.SqlDb.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.User.User, User>();
            CreateMap<User, Domain.User.User>();
        }
    }
}
