using AutoMapper;
using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
