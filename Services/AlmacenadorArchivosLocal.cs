namespace PeliculasApi.Services;

public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AlmacenadorArchivosLocal(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
    {
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var folder = Path.Combine(_environment.WebRootPath, contenedor);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var rutaGuardado = Path.Combine(folder, nombreArchivo);

        await File.WriteAllBytesAsync(rutaGuardado, contenido);
        var urlActual =
            $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

        var rutaParaBd = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");
        return rutaParaBd;
    }

    public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta,
        string contentType)
    {
        await BorrarArchivo(ruta, contenedor);
        return await GuardarArchivo(contenido, extension, contenedor, contentType);
    }

    public Task BorrarArchivo(string ruta, string contenedor)
    {
        if (ruta is null) return Task.FromResult(0);
        
        var nombreArchivo = Path.GetFileName(ruta);
        var directorioArchivo = Path.Combine(_environment.WebRootPath, contenedor, nombreArchivo);
        if (File.Exists(directorioArchivo)) File.Delete(directorioArchivo);

        return Task.FromResult(0);
    }
}