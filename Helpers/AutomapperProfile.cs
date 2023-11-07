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
// te traes las relaciones de la pelicula y las mapeas
            CreateMap<Pelicula, PeliculaDetalleDto>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculaGenero))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculaActor))
                ;

// agregar los generos y actores a la pelicula
            CreateMap<PeliculaCreacionDto, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                // con esto en la creacion ya se hacen las relaciones tanto de actores como de generos
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculaGenero))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculaActores))
                ;
        }

        private static List<GeneroDto> MapPeliculaGenero(Pelicula pelicula, PeliculaDetalleDto peliculaDetalleDto)
        {
            var result = new List<GeneroDto>();
            if (pelicula.PeliculasGeneros is null) return result;

            result.AddRange(pelicula.PeliculasGeneros.Select(genero => new GeneroDto()
                { Id = genero.GeneroId, Nombre = genero.Genero.Nombre }));

            return result;
        }

        private static List<ActorPeliculaDetalleDto> MapPeliculaActor(Pelicula pelicula,
            PeliculaDetalleDto peliculaDetalleDto)
        {
            var result = new List<ActorPeliculaDetalleDto>();
            if (pelicula.PeliculasActores is null) return result;

            result.AddRange(pelicula.PeliculasActores.Select(actor => new ActorPeliculaDetalleDto()
                { ActorId = actor.ActorId, NombrePersona = actor.Actor.Nombre, Personaje = actor.Personaje }));

            return result;
        }

        private static List<PeliculaGenero> MapPeliculaGenero(PeliculaCreacionDto peliculaCreacionDto,
            Pelicula pelicula)
        {
            var resultado = new List<PeliculaGenero>();
            if (peliculaCreacionDto.GenerosIDs is null) return resultado;
            resultado.AddRange(peliculaCreacionDto.GenerosIDs.Select(id => new PeliculaGenero() { GeneroId = id }));

            return resultado;
        }

        private static List<PeliculaActor> MapPeliculaActores(PeliculaCreacionDto peliculaCreacionDto,
            Pelicula pelicula)
        {
            var resultado = new List<PeliculaActor>();
            if (peliculaCreacionDto.Actores is null) return resultado;

            resultado.AddRange(peliculaCreacionDto.Actores.Select(actor => new PeliculaActor()
                { ActorId = actor.ActorId, Personaje = actor.Personaje }));

            return resultado;
        }
    }
}