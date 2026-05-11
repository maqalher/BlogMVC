using System;
using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Datos;

public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ApplicationDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // trae los comentarios que no han sido borrados
        builder.Entity<Comentario>().HasQueryFilter(x => !x.Borrado);
        // trae los entradas que no han sido borrados
        builder.Entity<Entrada>().HasQueryFilter(x => !x.Borrado);
    }

    public DbSet<Entrada> Entradas {get; set;}
    public DbSet<Comentario> Comentarios {get; set;}
    public DbSet<Lote> Lotes {get; set;}
}
