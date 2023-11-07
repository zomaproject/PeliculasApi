namespace PeliculasApi.DTOS;

public class PeliculaDetalleDto: PeliculaDto
{
    public List<GeneroDto> Generos { get; set; }
    public List<ActorPeliculaDetalleDto> Actores { get; set; }
}