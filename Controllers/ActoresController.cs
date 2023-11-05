using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Helpers;
using PeliculasApi.Properties.Entities;
using PeliculasApi.Services;

namespace PeliculasApi.Controllers;

[ApiController]
[Route("api/actores")]
public class ActoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;
    private readonly string contenedor = "actores";

    public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
    {
        _context = context;
        _mapper = mapper;
        _almacenadorArchivos = almacenadorArchivos;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDto>>> Get([FromQuery] PaginacionDto paginacionDto)
    {
        var queryable = _context.Actores.AsQueryable();
        await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDto.CantidadRegistrosPorPagina);

        var actoresDb = await queryable.Paginar(paginacionDto).ToListAsync();
        return _mapper.Map<List<ActorDto>>(actoresDb);
    }

    [HttpGet("id:int", Name = "obtenerActor")]
    public async Task<ActionResult<ActorDto>> Get(int id)
    {
        var entidad = await _context.Actores.FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (entidad is null) return NotFound();
        return _mapper.Map<ActorDto>(entidad);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] ActorCreacionDto actorCreacionDto)
    {
        var entidad = _mapper.Map<Actor>(actorCreacionDto);
        if (actorCreacionDto.Foto is not null)
        {
            using var memoryStream = new MemoryStream();
            await actorCreacionDto.Foto.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(actorCreacionDto.Foto.FileName);
            entidad.Foto = await _almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                actorCreacionDto.Foto.ContentType);
        }

        _context.Add(entidad);
        await _context.SaveChangesAsync();
        var entidadDto = _mapper.Map<ActorDto>(entidad);
        return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, entidadDto);
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await _context.Actores.AnyAsync(actorDb => actorDb.Id == id);
        if (!existe) return NotFound();
        _context.Remove(new Actor() { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDto actorCreacionDto)
    {
        var actorDb = await _context.Actores.FirstOrDefaultAsync(actorDb => actorDb.Id == id);
        if (actorDb is null) return NotFound();

        // Tomar los campos de actorCreacionDto y ponerlos en actorDb solo si son diferentes
        actorDb = _mapper.Map(actorCreacionDto, actorDb);


        if (actorCreacionDto.Foto is not null)
        {
            using var memoryStream = new MemoryStream();
            await actorCreacionDto.Foto.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(actorCreacionDto.Foto.FileName);
            actorDb.Foto = await _almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDb.Foto,
                actorCreacionDto.Foto.ContentType);
        }

        await _context.SaveChangesAsync();


        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDto> patchDocument)
    {
        if (patchDocument is null) return BadRequest();

        var entidadDb = await _context.Actores.FirstOrDefaultAsync(actorDb => actorDb.Id == id);

        if (entidadDb is null) return NotFound();

        var entidadDto = _mapper.Map<ActorPatchDto>(entidadDb);

        patchDocument.ApplyTo(entidadDto, ModelState);

        var esValido = TryValidateModel(entidadDto);
        if (!esValido) return BadRequest(ModelState);

        _mapper.Map(entidadDto, entidadDb);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}