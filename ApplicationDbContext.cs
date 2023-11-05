using Microsoft.EntityFrameworkCore;
using PeliculasApi.Properties.Entities;

namespace PeliculasApi;

public class ApplicationDbContext : DbContext
{
    public DbSet<Genero> Generos { get; set; }

    public DbSet<Actor> Actores { get; set; }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}