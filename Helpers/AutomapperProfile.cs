
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
            CreateMap<Genero, GeneroCreacionDto>().ReverseMap();
            CreateMap<Actor, ActorDto>().ReverseMap();
            CreateMap<ActorCreacionDto, Actor>();
        }
    }
}