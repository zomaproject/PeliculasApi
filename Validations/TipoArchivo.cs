using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Routing;

namespace PeliculasApi.Validations;

public class TipoArchivo : ValidationAttribute
{
    private readonly string[] _tiposValidos;

    public TipoArchivo(string[] tiposValidos)
    {
        _tiposValidos = tiposValidos;
    }

    public TipoArchivo(GrupoTIpoArchivo grupoTIpoArchivo)
    {
        if (grupoTIpoArchivo == GrupoTIpoArchivo.Imagen)
        {
            _tiposValidos = new[] { "image/jpeg", "image/png", "image/gif" };
        }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not IFormFile formFile) return ValidationResult.Success;
        if (!_tiposValidos.Contains(formFile.ContentType))
            return new ValidationResult(
                $"El tipo de archivo debe ser uno de los siguientes: {string.Join(", ", _tiposValidos)}");
        return ValidationResult.Success;
    }
}