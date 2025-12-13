using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIGDEF.Entidades;
using SIGDEF.Entidades.Enums;

namespace SIGDEF.AccesoDatos;

public class SIGDeFContext : DbContext
{
    public SIGDeFContext(DbContextOptions<SIGDeFContext> options) : base(options) { }

    // DbSet para todas las entidades
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Federacion> Federaciones { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<DelegadoClub> DelegadosClub { get; set; }
    public DbSet<Entrenador> Entrenadores { get; set; }
    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Atleta> Atletas { get; set; }
    public DbSet<AtletaTutor> AtletasTutores { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Inscripcion> Inscripciones { get; set; }
    public DbSet<PagoTransaccion> PagosTransacciones { get; set; }
    public DbSet<EventoPrueba> EventoPruebas { get; set; }
    public DbSet<DocumentacionPersona> DocumentacionPersonas { get; set; }


    public class SIGDeFContextFactory : IDesignTimeDbContextFactory<SIGDeFContext>
    {
        public SIGDeFContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SIGDeFContext>();

            // Usa tu cadena de conexión de PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sigdef_db;Username=sigdef_user;Password=Admin2508");

            return new SIGDeFContext(optionsBuilder.Options);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🔹 CONFIGURAR TODOS LOS DateTime PARA UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }

        // 🔹 CONFIGURACIÓN ESPECÍFICA PARA EL ENUM RolTipo
        modelBuilder.Entity<Rol>()
            .Property(r => r.Tipo)
            .HasConversion<string>() // Guarda el nombre del enum como string en la DB
            .HasMaxLength(50);       // Longitud adecuada para los nombres

        // 🔹 CONFIGURACIONES DE ELIMINACIÓN EN CASCADA

        // 🔹 Relaciones uno a uno con eliminación en cascada
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Persona)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Usuario>(u => u.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DelegadoClub>()
            .HasOne(d => d.Persona)
            .WithOne(p => p.DelegadoClub)
            .HasForeignKey<DelegadoClub>(d => d.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DelegadoClub>()
              .HasOne(d => d.Club)
              .WithMany()
              .HasForeignKey(d => d.ClubIdClub) 
              .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Entrenador>()
            .HasOne(e => e.Persona)
            .WithOne(p => p.Entrenador)
            .HasForeignKey<Entrenador>(e => e.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.Persona)
            .WithOne(p => p.Tutor)
            .HasForeignKey<Tutor>(t => t.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Atleta>()
            .HasOne(a => a.Persona)
            .WithOne(p => p.Atleta)
            .HasForeignKey<Atleta>(a => a.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Clave compuesta para AtletaTutor con eliminación en cascada
        modelBuilder.Entity<AtletaTutor>()
            .HasKey(at => new { at.IdAtleta, at.IdTutor });

        modelBuilder.Entity<AtletaTutor>()
            .HasOne(at => at.Atleta)
            .WithMany(a => a.Tutores)
            .HasForeignKey(at => at.IdAtleta)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AtletaTutor>()
            .HasOne(at => at.Tutor)
            .WithMany(t => t.AtletasTutores)
            .HasForeignKey(at => at.IdTutor)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Relaciones uno a muchos con eliminación en cascada
        modelBuilder.Entity<Inscripcion>()
            .HasOne(i => i.Atleta)
            .WithMany(a => a.Inscripciones)
            .HasForeignKey(i => i.IdAtleta)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Inscripcion>()
            .HasOne(i => i.EventoPrueba)
            .WithMany()
            .HasForeignKey(i => i.IdEventoPrueba)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PagoTransaccion>()
            .HasOne(p => p.Persona)
            .WithMany(p => p.Pagos)
            .HasForeignKey(p => p.IdPersona)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PagoTransaccion>()
            .HasOne(p => p.Club)
            .WithMany(c => c.Pagos)
            .HasForeignKey(p => p.IdClub)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DelegadoClub>()
            .HasOne(d => d.Rol)
            .WithMany(r => r.DelegadosClub)
            .HasForeignKey(d => d.IdRol)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DelegadoClub>()
            .HasOne(d => d.Federacion)
            .WithMany(f => f.DelegadosClub)
            .HasForeignKey(d => d.IdFederacion)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DelegadoClub>()
            .HasOne(d => d.Club)
            .WithMany(c => c.Representantes)
            .HasForeignKey(d => d.ClubIdClub)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Entrenador>()
            .HasOne(e => e.Club)
            .WithMany(c => c.Entrenadores)
            .HasForeignKey(e => e.IdClub)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Atleta>()
            .HasOne(a => a.Club)
            .WithMany(c => c.Atletas)
            .HasForeignKey(a => a.IdClub)
            .OnDelete(DeleteBehavior.Cascade);

       modelBuilder.Entity<Evento>()
            .HasMany(e => e.Pruebas)
            .WithOne(ed => ed.Evento)
            .HasForeignKey(ed => ed.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Mapeo de tablas (usa el mismo nombre que tus entidades, en singular)
        modelBuilder.Entity<Persona>().ToTable("Persona");
        modelBuilder.Entity<Usuario>().ToTable("Usuario");
        modelBuilder.Entity<Rol>().ToTable("Rol");
        modelBuilder.Entity<Federacion>().ToTable("Federacion");
        modelBuilder.Entity<Club>().ToTable("Club");
        modelBuilder.Entity<DelegadoClub>().ToTable("DelegadoClub");
        modelBuilder.Entity<Entrenador>().ToTable("Entrenador");
        modelBuilder.Entity<Tutor>().ToTable("Tutor");
        modelBuilder.Entity<Atleta>().ToTable("Atleta");
        modelBuilder.Entity<AtletaTutor>().ToTable("AtletaTutor");
        modelBuilder.Entity<Evento>().ToTable("Evento");
        modelBuilder.Entity<Inscripcion>().ToTable("Inscripcion");
        modelBuilder.Entity<PagoTransaccion>().ToTable("PagoTransaccion");
        modelBuilder.Entity<EventoPrueba>().ToTable("EventoPrueba");
    

    // 🔹 Conversión de enums a texto
    modelBuilder.Entity<Atleta>()
            .Property(a => a.EstadoPago)
            .HasConversion<string>();

        modelBuilder.Entity<Atleta>()
            .Property(a => a.Categoria)
            .HasConversion<string>();

        modelBuilder.Entity<AtletaTutor>()
            .Property(at => at.Parentesco)
            .HasConversion<string>();

        modelBuilder.Entity<PagoTransaccion>()
            .Property(p => p.Estado)
            .HasConversion<string>();

        // 🔹 Seed de roles CON EL ENUM ACTUALIZADO
        modelBuilder.Entity<Rol>().HasData(
            new Rol { IdRol = 1, Tipo = "Administrador" },
            new Rol { IdRol = 2, Tipo = "PresidenteFederacion" },
            new Rol { IdRol = 3, Tipo = "DelegadoClub" },
            new Rol { IdRol = 4, Tipo = "Entrenador" },
            new Rol { IdRol = 5, Tipo = "EntrenadorSeleccion" },
            new Rol { IdRol = 6, Tipo = "Atleta" },
            new Rol { IdRol = 7, Tipo = "Secretario" }
        );
         
        base.OnModelCreating(modelBuilder);
    }

    // Método adicional para obtener roles fácilmente
    public async Task<List<Rol>> GetRolesAsync()
    {
        return await Roles
            .OrderBy(r => r.IdRol)
            .ToListAsync();
    }



    // Método para validar si un ID de rol existe
    public async Task<bool> RolExistsAsync(int idRol)
    {
        return await Roles.AnyAsync(r => r.IdRol == idRol);
    }
}