using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Properties.Entities;

public class Pelicula
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(300)]
    public string Titulo { get; set; }
    
    public bool EnCines     { get; set; }
    
    public DateTime FechaEstreno { get; set; }
    
    public string Poster { get; set; }

    
    // Propiedades de navegación
    public List<PeliculaActor> PeliculasActores { get; set; }

    public List<PeliculaGenero> PeliculasGeneros { get; set; }
    //
}