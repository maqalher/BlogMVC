using System;

namespace BlogMVC.Models;

public class UsuariosRolesUsuarioViewModel
{
    public required string UsuarioId { get; set; }
    public required string Email { get; set; }
    public IEnumerable<UsuariosRolViewModel> Roles { get; set; } = [];
}
