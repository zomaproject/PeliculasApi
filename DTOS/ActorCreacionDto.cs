using System.ComponentModel.DataAnnotations;
using PeliculasApi.Validations;

namespace PeliculasApi.DTOS;

public class ActorCreacionDto
{
    [Required]
    [StringLength(120)]
    public string Nombre { get; set; }
    
    public DateTime FechaNacimiento { get; set; }

    [TipoArchivo(grupoTIpoArchivo: GrupoTIpoArchivo.Imagen)]
    [PesoArchivo(pesoMaximoMb: 4)]
    public IFormFile Foto { get; set; }
}