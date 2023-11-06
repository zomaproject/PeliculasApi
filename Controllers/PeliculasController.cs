using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Services;
using Pelicula = PeliculasApi.Properties.Entities.Pelicula;

namespace PeliculasApi.Controllers;

[ApiController]
[Route("api/peliculas")]
public class PeliculasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;
    private readonly string contenedor = "peliculas";

    public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
    {
        _context = context;
        _mapper = mapper;
        _almacenadorArchivos = almacenadorArchivos;
    }

    [HttpGet]
    public async Task<ActionResult<List<PeliculaDto>>> Get()
    {
        var peliculas = await _context.Peliculas.ToListAsync();
        
        return _mapper.Map<List<PeliculaDto>>(peliculas);
    }

    [HttpGet("id:int", Name = "obtenerPelicula")]
    public async Task<ActionResult<PeliculaDto>> Get(int id)
    {
        var entidad = await _context.Peliculas.FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (entidad is null) return NotFound();
        return _mapper.Map<PeliculaDto>(entidad);
    }
    [HttpPost]
    public async Task<ActionResult> Post([FromForm] PeliculaCreacionDto peliculaCreacionDto)
    {
        var entidad = _mapper.Map<Pelicula>(peliculaCreacionDto);
        if (peliculaCreacionDto.Poster is not null)
        {
            using var memoryStream = new MemoryStream();
            await peliculaCreacionDto.Poster.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(peliculaCreacionDto.Poster.FileName);
            entidad.Poster = await _almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                peliculaCreacionDto.Poster.ContentType);
        }

        _context.Add(entidad);
        await _context.SaveChangesAsync();
        var peliculaDto = _mapper.Map<PeliculaDto>(entidad);
        return new CreatedAtRouteResult("obtenerActor", new {id = entidad.Id}, peliculaDto);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDto peliculaCreacionDto)
    {
        var peliculaDb = await _context.Peliculas.FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (peliculaDb is null) return NotFound();

        // Tomar los campos de actorCreacionDto y ponerlos en actorDb solo si son diferentes
        peliculaDb = _mapper.Map(peliculaCreacionDto, peliculaDb);


        if (peliculaCreacionDto.Poster is not null)
        {
            using var memoryStream = new MemoryStream();
            await peliculaCreacionDto.Poster.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(peliculaCreacionDto.Poster.FileName);
            peliculaDb.Poster = await _almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDb.Poster,
                peliculaCreacionDto.Poster.ContentType);
        }

        await _context.SaveChangesAsync();


        return NoContent();
    }
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatch> patchDocument)
    {
        if (patchDocument is null) return BadRequest();

        var entidadDb = await _context.Peliculas.FirstOrDefaultAsync(actorDb => actorDb.Id == id);


        var entidadDto = _mapper.Map<PeliculaPatch>(entidadDb);

        patchDocument.ApplyTo(entidadDto, ModelState);

        var esValido = TryValidateModel(entidadDto);
        if (!esValido) return BadRequest(ModelState);

        _mapper.Map(entidadDto, entidadDb);

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await _context.Peliculas.AnyAsync(actorDb => actorDb.Id == id);
        if (!existe) return NotFound();
        _context.Remove(new Pelicula() { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

