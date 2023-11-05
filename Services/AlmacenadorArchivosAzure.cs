using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PeliculasApi.Services;

public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;


    public AlmacenadorArchivosAzure(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration["azureStorageConnectionString"];
    }

    public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
    {
        var cliente = new BlobContainerClient(_connectionString, contenedor);
        await cliente.CreateIfNotExistsAsync();
        await cliente.SetAccessPolicyAsync(PublicAccessType.Blob);

        var archivoNombre = $"{Guid.NewGuid()}{extension}";
        var blob = cliente.GetBlobClient(archivoNombre);
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };
        await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);
        return blob.Uri.ToString();
    }

    public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta,
        string contentType)
    {
        await BorrarArchivo(ruta, contenedor);
        return await GuardarArchivo(contenido, extension, contenedor, contentType);
    }

    public async Task BorrarArchivo(string ruta, string contenedor)
    {
        if (string.IsNullOrEmpty(ruta)) return;
        var cliente = new BlobContainerClient(_connectionString, contenedor);
        await cliente.CreateIfNotExistsAsync();
        var archivo = Path.GetFileName(ruta);
        var blob = cliente.GetBlobClient(archivo);
        await blob.DeleteIfExistsAsync();
    }
}