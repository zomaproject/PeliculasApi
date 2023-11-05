using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Validations;

public class PesoArchivo : ValidationAttribute
{
    private readonly int _pesoMaximoMb;

    public PesoArchivo(int pesoMaximoMb)
    {
        _pesoMaximoMb = pesoMaximoMb;
    }


    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not IFormFile formFile) return ValidationResult.Success;

        return formFile.Length > _pesoMaximoMb * 1024 * 1024
            ? new ValidationResult($"El peso del archivo no debe ser mayor a {_pesoMaximoMb}mb")
            : ValidationResult.Success;
    }
}