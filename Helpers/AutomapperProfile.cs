using AutoMapper;
using PeliculasApi.DTOS;
using PeliculasApi.Properties.Entities;

namespace PeliculasApi.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Genero, GeneroDto>().ReverseMap();

            CreateMap<GeneroCreacionDto, GeneroDto>();

            CreateMap<Actor, ActorDto>().ReverseMap();

            CreateMap<ActorCreacionDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());

            CreateMap<ActorPatchDto, Actor>().ReverseMap();
        }
    }
}