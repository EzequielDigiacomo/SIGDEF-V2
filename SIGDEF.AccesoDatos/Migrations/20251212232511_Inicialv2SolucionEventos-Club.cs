using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIGDEF.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class Inicialv2SolucionEventosClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Club",
                columns: table => new
                {
                    IdClub = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Siglas = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.IdClub);
                });

            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    IdEvento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TipoEvento = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaInicioInscripciones = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFinInscripciones = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ubicacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrecioBase = table.Column<decimal>(type: "numeric", nullable: false),
                    CupoMaximo = table.Column<int>(type: "integer", nullable: false),
                    TieneCronometraje = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereCertificadoMedico = table.Column<bool>(type: "boolean", nullable: false),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.IdEvento);
                });

            migrationBuilder.CreateTable(
                name: "Federacion",
                columns: table => new
                {
                    IdFederacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cuit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BancoNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoCuenta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NumeroCuenta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TitularCuenta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailCobro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Federacion", x => x.IdFederacion);
                });

            migrationBuilder.CreateTable(
                name: "Persona",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Documento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Sexo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persona", x => x.IdPersona);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "EventoPrueba",
                columns: table => new
                {
                    IdEventoPrueba = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEvento = table.Column<int>(type: "integer", nullable: false),
                    Distancia = table.Column<int>(type: "integer", nullable: false),
                    CategoriaEdad = table.Column<int>(type: "integer", nullable: false),
                    SexoCompetencia = table.Column<int>(type: "integer", nullable: false),
                    TipoBote = table.Column<int>(type: "integer", nullable: false),
                    PrecioCategoria = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPrueba", x => x.IdEventoPrueba);
                    table.ForeignKey(
                        name: "FK_EventoPrueba_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "IdEvento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atleta",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: false),
                    EstadoPago = table.Column<string>(type: "text", nullable: false),
                    PerteneceSeleccion = table.Column<bool>(type: "boolean", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BecadoEnard = table.Column<bool>(type: "boolean", nullable: false),
                    BecadoSdn = table.Column<bool>(type: "boolean", nullable: false),
                    MontoBeca = table.Column<decimal>(type: "numeric", nullable: false),
                    PresentoAptoMedico = table.Column<bool>(type: "boolean", nullable: false),
                    FechaAptoMedico = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atleta", x => x.IdPersona);
                    table.ForeignKey(
                        name: "FK_Atleta_Club_IdClub",
                        column: x => x.IdClub,
                        principalTable: "Club",
                        principalColumn: "IdClub",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atleta_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentacionPersonas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonaId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumento = table.Column<int>(type: "integer", nullable: false),
                    UrlArchivo = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentacionPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentacionPersonas_Persona_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entrenador",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: false),
                    Licencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerteneceSeleccion = table.Column<bool>(type: "boolean", nullable: false),
                    CategoriaSeleccion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BecadoEnard = table.Column<bool>(type: "boolean", nullable: false),
                    BecadoSdn = table.Column<bool>(type: "boolean", nullable: false),
                    MontoBeca = table.Column<decimal>(type: "numeric", nullable: false),
                    PresentoAptoMedico = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrenador", x => x.IdPersona);
                    table.ForeignKey(
                        name: "FK_Entrenador_Club_IdClub",
                        column: x => x.IdClub,
                        principalTable: "Club",
                        principalColumn: "IdClub",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entrenador_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagoTransaccion",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Concepto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: false),
                    IdMercadoPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoTransaccion", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_PagoTransaccion_Club_IdClub",
                        column: x => x.IdClub,
                        principalTable: "Club",
                        principalColumn: "IdClub",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagoTransaccion_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tutor",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    TipoTutor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutor", x => x.IdPersona);
                    table.ForeignKey(
                        name: "FK_Tutor_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPersona = table.Column<int>(type: "integer", nullable: true),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false),
                    Rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Club_IdClub",
                        column: x => x.IdClub,
                        principalTable: "Club",
                        principalColumn: "IdClub");
                    table.ForeignKey(
                        name: "FK_Usuario_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DelegadoClub",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdFederacion = table.Column<int>(type: "integer", nullable: false),
                    ClubIdClub = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegadoClub", x => x.IdPersona);
                    table.ForeignKey(
                        name: "FK_DelegadoClub_Club_ClubIdClub",
                        column: x => x.ClubIdClub,
                        principalTable: "Club",
                        principalColumn: "IdClub",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegadoClub_Federacion_IdFederacion",
                        column: x => x.IdFederacion,
                        principalTable: "Federacion",
                        principalColumn: "IdFederacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegadoClub_Persona_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Persona",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegadoClub_Rol_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inscripcion",
                columns: table => new
                {
                    IdInscripcion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAtleta = table.Column<int>(type: "integer", nullable: false),
                    IdEventoPrueba = table.Column<int>(type: "integer", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventoIdEvento = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripcion", x => x.IdInscripcion);
                    table.ForeignKey(
                        name: "FK_Inscripcion_Atleta_IdAtleta",
                        column: x => x.IdAtleta,
                        principalTable: "Atleta",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripcion_EventoPrueba_IdEventoPrueba",
                        column: x => x.IdEventoPrueba,
                        principalTable: "EventoPrueba",
                        principalColumn: "IdEventoPrueba",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripcion_Evento_EventoIdEvento",
                        column: x => x.EventoIdEvento,
                        principalTable: "Evento",
                        principalColumn: "IdEvento");
                });

            migrationBuilder.CreateTable(
                name: "AtletaTutor",
                columns: table => new
                {
                    IdAtleta = table.Column<int>(type: "integer", nullable: false),
                    IdTutor = table.Column<int>(type: "integer", nullable: false),
                    Parentesco = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtletaTutor", x => new { x.IdAtleta, x.IdTutor });
                    table.ForeignKey(
                        name: "FK_AtletaTutor_Atleta_IdAtleta",
                        column: x => x.IdAtleta,
                        principalTable: "Atleta",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtletaTutor_Tutor_IdTutor",
                        column: x => x.IdTutor,
                        principalTable: "Tutor",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "IdRol", "Tipo" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "PresidenteFederacion" },
                    { 3, "DelegadoClub" },
                    { 4, "Entrenador" },
                    { 5, "EntrenadorSeleccion" },
                    { 6, "Atleta" },
                    { 7, "Secretario" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atleta_IdClub",
                table: "Atleta",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_AtletaTutor_IdTutor",
                table: "AtletaTutor",
                column: "IdTutor");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadoClub_ClubIdClub",
                table: "DelegadoClub",
                column: "ClubIdClub");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadoClub_IdFederacion",
                table: "DelegadoClub",
                column: "IdFederacion");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadoClub_IdRol",
                table: "DelegadoClub",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentacionPersonas_PersonaId",
                table: "DocumentacionPersonas",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenador_IdClub",
                table: "Entrenador",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPrueba_IdEvento",
                table: "EventoPrueba",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcion_EventoIdEvento",
                table: "Inscripcion",
                column: "EventoIdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcion_IdAtleta",
                table: "Inscripcion",
                column: "IdAtleta");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcion_IdEventoPrueba",
                table: "Inscripcion",
                column: "IdEventoPrueba");

            migrationBuilder.CreateIndex(
                name: "IX_PagoTransaccion_IdClub",
                table: "PagoTransaccion",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_PagoTransaccion_IdPersona",
                table: "PagoTransaccion",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdClub",
                table: "Usuario",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdPersona",
                table: "Usuario",
                column: "IdPersona",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtletaTutor");

            migrationBuilder.DropTable(
                name: "DelegadoClub");

            migrationBuilder.DropTable(
                name: "DocumentacionPersonas");

            migrationBuilder.DropTable(
                name: "Entrenador");

            migrationBuilder.DropTable(
                name: "Inscripcion");

            migrationBuilder.DropTable(
                name: "PagoTransaccion");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Tutor");

            migrationBuilder.DropTable(
                name: "Federacion");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Atleta");

            migrationBuilder.DropTable(
                name: "EventoPrueba");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.DropTable(
                name: "Persona");

            migrationBuilder.DropTable(
                name: "Evento");
        }
    }
}
