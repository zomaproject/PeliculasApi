using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Properties.Entities;

namespace PeliculasApi.Controllers;

[ApiController]
[Route("api/actores")]
public class ActoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ActoresController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDto>>> Get()
    {
        var actoresDb = await _context.Actores.ToListAsync();
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
    public async Task<ActionResult> Post( [FromForm] ActorCreacionDto actorCreacionDto)
    {
       var entidad = _mapper.Map<Actor>(actorCreacionDto);
       _context.Add(entidad);
       // await _context.SaveChangesAsync();
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


}