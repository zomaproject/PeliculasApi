namespace PeliculasApi.DTOS;

public class PeliculasIndexDto
{
    public List<PeliculaDto> FuturosEstrenos { get; set; }
    public List<PeliculaDto> PeliculasEnCines { get; set; }
}