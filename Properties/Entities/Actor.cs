﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Properties.Entities;

public class Actor
{
    public int Id { get; set; }

    [Required] [StringLength(120)] public string Nombre { get; set; }

    public DateTime FechaNacimiento { get; set; }
    public string Foto { get; set; }
}