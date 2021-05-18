using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TallerWebApi.Migrations
{
    public partial class allModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    CI = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ManoObra = table.Column<double>(nullable: false),
                    Impuesto = table.Column<double>(nullable: false),
                    DetalleManoObra = table.Column<string>(nullable: true),
                    DetalledeRespuesto = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    NotificadoCliente = table.Column<bool>(nullable: false),
                    EstadoPresupuesto = table.Column<int>(nullable: false),
                    InformeCliente = table.Column<string>(nullable: true),
                    Garantia = table.Column<bool>(nullable: false),
                    FechaGarantia = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repuestos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Precio = table.Column<double>(nullable: false),
                    PrecioUnitario = table.Column<double>(nullable: false),
                    Cantidad = table.Column<double>(nullable: false),
                    Aceptado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repuestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Talleres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talleres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    MarcaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modelos_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Cargo = table.Column<string>(nullable: true),
                    TallerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tecnicos_Talleres_TallerId",
                        column: x => x.TallerId,
                        principalTable: "Talleres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    NumeroSerie = table.Column<int>(nullable: false),
                    FechaFabricacion = table.Column<DateTime>(nullable: false),
                    TipoId = table.Column<int>(nullable: false),
                    MarcaId = table.Column<int>(nullable: false),
                    ModeloId = table.Column<int>(nullable: false),
                    ClienteId = table.Column<int>(nullable: false),
                    Observaciones = table.Column<string>(nullable: true),
                    SituacionEquipo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipos_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipos_Marcas_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipos_Tipos_TipoId",
                        column: x => x.TipoId,
                        principalTable: "Tipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    EquipoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoEquipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Accion = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Usuario = table.Column<string>(nullable: true),
                    EquipoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoEquipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoEquipos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesReparaciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    NoOrden = table.Column<int>(nullable: false),
                    ClienteId = table.Column<int>(nullable: false),
                    TecnicoRxEquipoId = table.Column<int>(nullable: false),
                    TecnicoId = table.Column<int>(nullable: false),
                    EquipoId = table.Column<int>(nullable: false),
                    Defecto = table.Column<string>(nullable: true),
                    Causa = table.Column<string>(nullable: true),
                    Accion = table.Column<string>(nullable: true),
                    DefectoCliente = table.Column<string>(nullable: true),
                    TiempoEmpleado = table.Column<double>(nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false),
                    FechaFinalizado = table.Column<DateTime>(nullable: false),
                    FechaCerrado = table.Column<DateTime>(nullable: false),
                    FechaPrometido = table.Column<DateTime>(nullable: false),
                    Garantía = table.Column<DateTime>(nullable: false),
                    EstadoOrden = table.Column<int>(nullable: false),
                    TallerId = table.Column<int>(nullable: false),
                    PresupuestoId = table.Column<int>(nullable: false),
                    InformeTecnico = table.Column<string>(nullable: true),
                    LugarReparacion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesReparaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Presupuestos_PresupuestoId",
                        column: x => x.PresupuestoId,
                        principalTable: "Presupuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Talleres_TallerId",
                        column: x => x.TallerId,
                        principalTable: "Talleres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Tecnicos_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Tecnicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Tecnicos_TecnicoRxEquipoId",
                        column: x => x.TecnicoRxEquipoId,
                        principalTable: "Tecnicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesReparaciones_Repuestos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrdenReparacionId = table.Column<int>(nullable: false),
                    RepuestoId = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesReparaciones_Repuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Repuestos_OrdenesReparaciones_OrdenRepa~",
                        column: x => x.OrdenReparacionId,
                        principalTable: "OrdenesReparaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesReparaciones_Repuestos_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RMAs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FechaEnvio = table.Column<DateTime>(nullable: false),
                    OrdenReparacionId = table.Column<int>(nullable: false),
                    ProveedorId = table.Column<int>(nullable: false),
                    EquipoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RMAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RMAs_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RMAs_OrdenesReparaciones_OrdenReparacionId",
                        column: x => x.OrdenReparacionId,
                        principalTable: "OrdenesReparaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RMAs_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_EquipoId",
                table: "Documentos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_ClienteId",
                table: "Equipos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_MarcaId",
                table: "Equipos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_ModeloId",
                table: "Equipos",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_TipoId",
                table: "Equipos",
                column: "TipoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoEquipos_EquipoId",
                table: "HistoricoEquipos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modelos_MarcaId",
                table: "Modelos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_ClienteId",
                table: "OrdenesReparaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_EquipoId",
                table: "OrdenesReparaciones",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_PresupuestoId",
                table: "OrdenesReparaciones",
                column: "PresupuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_TallerId",
                table: "OrdenesReparaciones",
                column: "TallerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_TecnicoId",
                table: "OrdenesReparaciones",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_TecnicoRxEquipoId",
                table: "OrdenesReparaciones",
                column: "TecnicoRxEquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_Repuestos_OrdenReparacionId",
                table: "OrdenesReparaciones_Repuestos",
                column: "OrdenReparacionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReparaciones_Repuestos_RepuestoId",
                table: "OrdenesReparaciones_Repuestos",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_RMAs_EquipoId",
                table: "RMAs",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_RMAs_OrdenReparacionId",
                table: "RMAs",
                column: "OrdenReparacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RMAs_ProveedorId",
                table: "RMAs",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_TallerId",
                table: "Tecnicos",
                column: "TallerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "HistoricoEquipos");

            migrationBuilder.DropTable(
                name: "Modelos");

            migrationBuilder.DropTable(
                name: "OrdenesReparaciones_Repuestos");

            migrationBuilder.DropTable(
                name: "RMAs");

            migrationBuilder.DropTable(
                name: "Repuestos");

            migrationBuilder.DropTable(
                name: "OrdenesReparaciones");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Presupuestos");

            migrationBuilder.DropTable(
                name: "Tecnicos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "Tipos");

            migrationBuilder.DropTable(
                name: "Talleres");
        }
    }
}
