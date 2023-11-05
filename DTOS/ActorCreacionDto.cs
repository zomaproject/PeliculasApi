using System.ComponentModel.DataAnnotations;
using PeliculasApi.Validations;

namespace PeliculasApi.DTOS;

public class ActorCreacionDto: ActorPatchDto
{

    [TipoArchivo(grupoTIpoArchivo: GrupoTIpoArchivo.Imagen)]
    [PesoArchivo(pesoMaximoMb: 4)]
    public IFormFile Foto { get; set; }
}