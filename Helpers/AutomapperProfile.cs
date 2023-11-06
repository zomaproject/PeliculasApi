using AutoMapper;
using PeliculasApi.DTOS;
using PeliculasApi.Properties.Entities;
using Pelicula = PeliculasApi.Properties.Entities.Pelicula;

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

            CreateMap<Pelicula, PeliculaDto>().ReverseMap();


            CreateMap<PeliculaPatch, Pelicula>().ReverseMap();

            CreateMap<PeliculaCreacionDto, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                // con esto en la creacion ya se hacen las relaciones tanto de actores como de generos
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculaGenero))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculaActores))
                ;
        }


        private static List<PeliculaGenero> MapPeliculaGenero(PeliculaCreacionDto peliculaCreacionDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculaGenero>();
            if (peliculaCreacionDto.GenerosIDs is null) return resultado;
            resultado.AddRange(peliculaCreacionDto.GenerosIDs.Select(id => new PeliculaGenero() { GeneroId = id }));

            return resultado;
        }

        private static List<PeliculaActor> MapPeliculaActores(PeliculaCreacionDto peliculaCreacionDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculaActor>();
            if (peliculaCreacionDto.Actores is null) return resultado;

            resultado.AddRange(peliculaCreacionDto.Actores.Select(actor => new PeliculaActor()
                { ActorId = actor.ActorId, Personaje = actor.Personaje }));

            return resultado;
        }
    }
}