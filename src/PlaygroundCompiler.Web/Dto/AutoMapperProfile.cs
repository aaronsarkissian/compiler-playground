using AutoMapper;
using PlaygroundCompiler.Infrastrucutre.Entities;

namespace PlaygroundCompiler.Web.Dto
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CompilerOutput, CompilerOutputDTO>()
                .ReverseMap();
        }
    }
}