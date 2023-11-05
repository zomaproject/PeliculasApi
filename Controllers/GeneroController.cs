using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Properties.Entities;

namespace PeliculasApi.Controllers;

[ApiController]
[Route("api/generos")]
public class GeneroController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeneroController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<GeneroDto>>> Get()
    {
        var generosDb = await _context.Generos.ToListAsync();
        return _mapper.Map<List<GeneroDto>>(generosDb);
    }

    [HttpGet("{id:int}", Name = "obtenerGenero")]
    public async Task<ActionResult<GeneroDto>> Get(int id)
    {
        var generoDb = await _context.Generos.FirstOrDefaultAsync(generoDb => generoDb.Id == id);
        if (generoDb is null) return NotFound();
        return _mapper.Map<GeneroDto>(generoDb);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] GeneroCreacionDto generoCreacionDto)
    {
        var generoDb = _mapper.Map<Genero>(generoCreacionDto);
        _context.Add(generoDb);
        await _context.SaveChangesAsync();
        var generoDto = _mapper.Map<GeneroDto>(generoDb);
        return new CreatedAtRouteResult("obtenerGenero", new { id = generoDb.Id }, generoDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDto generoCreacionDto)
    {
        var existe = await _context.Generos.AnyAsync(generoDb => generoDb.Id == id);
        if (!existe) return NotFound();

        var entidad = _mapper.Map<Genero>(generoCreacionDto);
        entidad.Id = id;
        _context.Entry(entidad).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await _context.Generos.AnyAsync(generoDb => generoDb.Id == id);
        if (!existe) return NotFound();
        _context.Remove(new Genero { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}