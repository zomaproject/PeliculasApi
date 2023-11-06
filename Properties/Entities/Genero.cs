using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Properties.Entities;

public class Genero
{
    public int Id { get; set; }
    [Required] [StringLength(40)] public string Nombre { get; set; }
    
    
    // Propiedades de navegación
    public List<PeliculaGenero> PeliculasGeneros { get; set; }
}