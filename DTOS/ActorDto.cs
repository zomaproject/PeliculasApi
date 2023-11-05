﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS;

public class ActorDto
{
    public int Id { get; set; }
    [Required] [StringLength(120)] public string Nombre { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Foto { get; set; }
}