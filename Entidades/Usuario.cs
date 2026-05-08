using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlogMVC.Entidades;

public class Usuario: IdentityUser
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
}
