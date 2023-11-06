namespace PeliculasApi.Properties.Entities;

public class PeliculaGenero
{
    public int GeneroId { get; set; }
    public int PeliculaId { get; set; }
    
    public Pelicula Pelicula { get; set; }
    public Genero Genero { get; set; }
}