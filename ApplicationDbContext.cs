using Microsoft.EntityFrameworkCore;
using PeliculasApi.Properties.Entities;

namespace PeliculasApi;

public class ApplicationDbContext : DbContext
{
    public DbSet<Genero> Generos { get; set; }

    public DbSet<Actor> Actores { get; set; }

    public DbSet<Pelicula> Peliculas { get; set; }

    public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
    public DbSet<PeliculaActor> PeliculaActores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PeliculaActor>().HasKey(x => new { x.ActorId, x.PeliculaId });
        modelBuilder.Entity<PeliculaGenero>().HasKey(x => new { x.GeneroId, x.PeliculaId });

        base.OnModelCreating(modelBuilder);
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}