using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Helpers;
using PeliculasApi.Services;
using Pelicula = PeliculasApi.Properties.Entities.Pelicula;
using System.Linq.Dynamic.Core;

namespace PeliculasApi.Controllers;

[ApiController]
[Route("api/peliculas")]
public class PeliculasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;
    private readonly ILogger<PeliculasController> _logger;
    private readonly string _contenedor = "peliculas";

    public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculasController> logger)
    {
        _context = context;
        _mapper = mapper;
        _almacenadorArchivos = almacenadorArchivos;
        _logger = logger;
    }

    [HttpGet]
    public async Task<PeliculasIndexDto> Get()
    {
        const int top = 5;

        var hoy = DateTime.Today;
        var proximosEstrenos = await _context.Peliculas
            .Where(x => x.FechaEstreno > hoy)
            .OrderBy(x => x.FechaEstreno)
            .Take(top)
            .ToListAsync();


        var enCines = await _context.Peliculas
            .Where(x => x.EnCines)
            .OrderBy(x => x.FechaEstreno)
            .Take(top)
            .ToListAsync();

        var enCinesDto = _mapper.Map<List<PeliculaDto>>(enCines);
        var proximosEstrenosDto = _mapper.Map<List<PeliculaDto>>(proximosEstrenos);
        var result = new PeliculasIndexDto() { PeliculasEnCines = enCinesDto, FuturosEstrenos = proximosEstrenosDto };

        return result;
    }

    [HttpGet("{id:int}", Name = "obtenerPelicula")]
    public async Task<ActionResult<PeliculaDetalleDto>> Get(int id)
    {
        var peliculaDb = await _context.Peliculas
            .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
            .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
            .FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (peliculaDb is null) return NotFound();

        peliculaDb.PeliculasActores = peliculaDb.PeliculasActores.OrderBy(x => x.Orden).ToList();
        return _mapper.Map<PeliculaDetalleDto>(peliculaDb);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] PeliculaCreacionDto peliculaCreacionDto)
    {
        var peliculaDb = _mapper.Map<Pelicula>(peliculaCreacionDto);
        if (peliculaCreacionDto.Poster is not null)
        {
            using var memoryStream = new MemoryStream();
            await peliculaCreacionDto.Poster.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(peliculaCreacionDto.Poster.FileName);
            peliculaDb.Poster = await _almacenadorArchivos.GuardarArchivo(contenido, extension, _contenedor,
                peliculaCreacionDto.Poster.ContentType);
        }

        AsignarOrdenActores(peliculaDb);
        _context.Add(peliculaDb);
        await _context.SaveChangesAsync();
        var peliculaDto = _mapper.Map<PeliculaDto>(peliculaDb);
        return new CreatedAtRouteResult("obtenerActor", new { id = peliculaDb.Id }, peliculaDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDto peliculaCreacionDto)
    {
        var peliculaDb = await _context.Peliculas
            .Include(peliculaDb => peliculaDb.PeliculasActores)
            .Include(peliculaDb => peliculaDb.PeliculasGeneros)
            .FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (peliculaDb is null) return NotFound();

        // Tomar los campos de actorCreacionDto y ponerlos en actorDb solo si son diferentes
        peliculaDb = _mapper.Map(peliculaCreacionDto, peliculaDb);


        if (peliculaCreacionDto.Poster is not null)
        {
            using var memoryStream = new MemoryStream();
            await peliculaCreacionDto.Poster.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(peliculaCreacionDto.Poster.FileName);
            peliculaDb.Poster = await _almacenadorArchivos.EditarArchivo(contenido, extension, _contenedor,
                peliculaDb.Poster,
                peliculaCreacionDto.Poster.ContentType);
        }

        AsignarOrdenActores(peliculaDb);
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

    private static void AsignarOrdenActores(Pelicula pelicula)
    {
        if (pelicula.PeliculasActores is null) return;
        for (var i = 0; i < pelicula.PeliculasActores.Count; i++)
        {
            pelicula.PeliculasActores[i].Orden = i;
        }
    }


    [HttpGet("filtro")]
    public async Task<ActionResult<List<PeliculaDto>>> Filtrar([FromQuery] FitlroPeliculaDto fitlroPeliculaDto)
    {
        var peliculasQueryable = _context.Peliculas.AsQueryable();
        if (!string.IsNullOrEmpty(fitlroPeliculaDto.Titulo))
        {
            peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(fitlroPeliculaDto.Titulo));
        }

        if (fitlroPeliculaDto.EnCines)
        {
            peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
        }

        if (fitlroPeliculaDto.ProximosEstrenos)
        {
            var hoy = DateTime.Today;
            peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
        }

        if (fitlroPeliculaDto.GeneroId != 0)
        {
            peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                .Contains(fitlroPeliculaDto.GeneroId));
        }

        if (!string.IsNullOrEmpty(fitlroPeliculaDto.CampoOrdenar))
        {
            var tipoOrden = fitlroPeliculaDto.Ascendente ? "ascending" : "descending";

            try
            {
                peliculasQueryable = peliculasQueryable.OrderBy($"{fitlroPeliculaDto.CampoOrdenar} {tipoOrden}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
            fitlroPeliculaDto.CantidadRegistrosPorPagina);
        var peliculas = await peliculasQueryable.Paginar(fitlroPeliculaDto.PaginacionDto).ToListAsync();
        return _mapper.Map<List<PeliculaDto>>(peliculas);
    }
}