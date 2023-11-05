﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS;

public class GeneroCreacionDto
{

    [Required]
    [StringLength(40)]
    public string Nombre { get; set; }
}