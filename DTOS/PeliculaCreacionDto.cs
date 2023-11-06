using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Helpers;
using PeliculasApi.Validations;

namespace PeliculasApi.DTOS;

public class PeliculaCreacionDto : PeliculaPatch
{
    [PesoArchivo(pesoMaximoMb: 4)]
    [TipoArchivo(grupoTIpoArchivo: GrupoTIpoArchivo.Imagen)]
    public IFormFile Poster { get; set; }

    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> GenerosIDs { get; set; }

    [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaCreacionDto>>))]
    public List<ActorPeliculaCreacionDto> Actores { get; set; }
}