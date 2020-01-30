using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class estadosEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actividades_laborales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades_laborales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categorias_ocupacionales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_ocupacionales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CentroDeCosto = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    MontoCUC = table.Column<decimal>(nullable: false),
                    MontoCUP = table.Column<decimal>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: false),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaTerminado = table.Column<DateTime>(nullable: true),
                    Pagado = table.Column<bool>(nullable: false),
                    Sobregirado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "provincias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_de_unidad_organizativa",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: false),
                    Prioridad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_de_unidad_organizativa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "grupos_escalas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true),
                    CategoriaOcupacionalId = table.Column<int>(nullable: false),
                    SalarioDiferenciado = table.Column<bool>(nullable: false),
                    SalarioEscala = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos_escalas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_grupos_escalas_categorias_ocupacionales_CategoriaOcupaciona~",
                        column: x => x.CategoriaOcupacionalId,
                        principalTable: "categorias_ocupacionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "actividades_de_contratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ActividadLaboralId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false),
                    PrecioCUC = table.Column<decimal>(nullable: false),
                    PrecioCUP = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades_de_contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_actividades_de_contratos_actividades_laborales_ActividadLab~",
                        column: x => x.ActividadLaboralId,
                        principalTable: "actividades_laborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_actividades_de_contratos_contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "municipios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    ProvinciaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_municipios_provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "unidades_organizativas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(nullable: false),
                    TipoUnidadOrganizativaId = table.Column<int>(nullable: false),
                    PerteneceAId = table.Column<int>(nullable: true),
                    Activa = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unidades_organizativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_unidades_organizativas_unidades_organizativas_PerteneceAId",
                        column: x => x.PerteneceAId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unidades_organizativas_tipos_de_unidad_organizativa_TipoUni~",
                        column: x => x.TipoUnidadOrganizativaId,
                        principalTable: "tipos_de_unidad_organizativa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cargo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Sigla = table.Column<string>(nullable: true),
                    GrupoEscalaId = table.Column<int>(nullable: true),
                    JefeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cargo_grupos_escalas_GrupoEscalaId",
                        column: x => x.GrupoEscalaId,
                        principalTable: "grupos_escalas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cargo_cargo_JefeId",
                        column: x => x.JefeId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Plantilla",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CargoId = table.Column<int>(nullable: false),
                    UnidadOrganizativaId = table.Column<int>(nullable: false),
                    PlantillaAprobada = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plantilla_cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plantilla_unidades_organizativas_UnidadOrganizativaId",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "puestos_de_trabajos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CargoId = table.Column<int>(nullable: false),
                    UnidadOrganizativaId = table.Column<int>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    PlantillaOcupada = table.Column<int>(nullable: false),
                    JefeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puestos_de_trabajos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_puestos_de_trabajos_cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_puestos_de_trabajos_puestos_de_trabajos_JefeId",
                        column: x => x.JefeId,
                        principalTable: "puestos_de_trabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_puestos_de_trabajos_unidades_organizativas_UnidadOrganizati~",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trabajadores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    Apellidos = table.Column<string>(nullable: true),
                    CI = table.Column<string>(nullable: true),
                    TelefonoFijo = table.Column<string>(nullable: true),
                    TelefonoMovil = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    Sexo = table.Column<int>(nullable: false),
                    Direccion = table.Column<string>(nullable: true),
                    MunicipioId = table.Column<int>(nullable: true),
                    PuestoDeTrabajoId = table.Column<int>(nullable: true),
                    NivelDeEscolaridad = table.Column<int>(nullable: false),
                    EstadoTrabajador = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajadores_municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajadores_puestos_de_trabajos_PuestoDeTrabajoId",
                        column: x => x.PuestoDeTrabajoId,
                        principalTable: "puestos_de_trabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bajas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TrabajadorId = table.Column<int>(nullable: false),
                    CausaDeBaja = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bajas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bajas_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "caracteristicas_del_trabjador",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrabajadorId = table.Column<int>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    ColorDePiel = table.Column<int>(nullable: false),
                    ColorDeOjos = table.Column<int>(nullable: false),
                    TallaPantalon = table.Column<string>(nullable: true),
                    TallaDeCamisa = table.Column<int>(nullable: false),
                    TallaCalzado = table.Column<double>(nullable: false),
                    OtrasCaracteristicas = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caracteristicas_del_trabjador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_caracteristicas_del_trabjador_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TrabajadorId = table.Column<int>(nullable: false),
                    CargoId = table.Column<int>(nullable: false),
                    UnidadOrganizativaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entradas_cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entradas_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entradas_unidades_organizativas_UnidadOrganizativaId",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historicos_de_puestos_de_trabajos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrabajadorId = table.Column<int>(nullable: false),
                    PuestoDeTrabajoId = table.Column<int>(nullable: true),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaTerminado = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historicos_de_puestos_de_trabajos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historicos_de_puestos_de_trabajos_puestos_de_trabajos_Puest~",
                        column: x => x.PuestoDeTrabajoId,
                        principalTable: "puestos_de_trabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historicos_de_puestos_de_trabajos_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtrosMovimientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TrabajadorId = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(nullable: true),
                    Estado = table.Column<int>(nullable: false),
                    Desde = table.Column<DateTime>(nullable: false),
                    Hasta = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtrosMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtrosMovimientos_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Traslados",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TrabajadorId = table.Column<int>(nullable: false),
                    CargoOrigenId = table.Column<int>(nullable: true),
                    CargoDestinoId = table.Column<int>(nullable: false),
                    UnidadOrganizativaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traslados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traslados_cargo_CargoDestinoId",
                        column: x => x.CargoDestinoId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traslados_cargo_CargoOrigenId",
                        column: x => x.CargoOrigenId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Traslados_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traslados_unidades_organizativas_UnidadOrganizativaId",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_de_contratos_ActividadLaboralId",
                table: "actividades_de_contratos",
                column: "ActividadLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_actividades_de_contratos_ContratoId",
                table: "actividades_de_contratos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_bajas_TrabajadorId",
                table: "bajas",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_del_trabjador_TrabajadorId",
                table: "caracteristicas_del_trabjador",
                column: "TrabajadorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cargo_GrupoEscalaId",
                table: "cargo",
                column: "GrupoEscalaId");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_JefeId",
                table: "cargo",
                column: "JefeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_CargoId",
                table: "Entradas",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_TrabajadorId",
                table: "Entradas",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_UnidadOrganizativaId",
                table: "Entradas",
                column: "UnidadOrganizativaId");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_escalas_CategoriaOcupacionalId",
                table: "grupos_escalas",
                column: "CategoriaOcupacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_historicos_de_puestos_de_trabajos_PuestoDeTrabajoId",
                table: "historicos_de_puestos_de_trabajos",
                column: "PuestoDeTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_historicos_de_puestos_de_trabajos_TrabajadorId",
                table: "historicos_de_puestos_de_trabajos",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_municipios_ProvinciaId",
                table: "municipios",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosMovimientos_TrabajadorId",
                table: "OtrosMovimientos",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantilla_CargoId",
                table: "Plantilla",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantilla_UnidadOrganizativaId",
                table: "Plantilla",
                column: "UnidadOrganizativaId");

            migrationBuilder.CreateIndex(
                name: "IX_puestos_de_trabajos_CargoId",
                table: "puestos_de_trabajos",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_puestos_de_trabajos_JefeId",
                table: "puestos_de_trabajos",
                column: "JefeId");

            migrationBuilder.CreateIndex(
                name: "IX_puestos_de_trabajos_UnidadOrganizativaId",
                table: "puestos_de_trabajos",
                column: "UnidadOrganizativaId");

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_MunicipioId",
                table: "trabajadores",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_PuestoDeTrabajoId",
                table: "trabajadores",
                column: "PuestoDeTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_CargoDestinoId",
                table: "Traslados",
                column: "CargoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_CargoOrigenId",
                table: "Traslados",
                column: "CargoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_TrabajadorId",
                table: "Traslados",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_UnidadOrganizativaId",
                table: "Traslados",
                column: "UnidadOrganizativaId");

            migrationBuilder.CreateIndex(
                name: "IX_unidades_organizativas_PerteneceAId",
                table: "unidades_organizativas",
                column: "PerteneceAId");

            migrationBuilder.CreateIndex(
                name: "IX_unidades_organizativas_TipoUnidadOrganizativaId",
                table: "unidades_organizativas",
                column: "TipoUnidadOrganizativaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actividades_de_contratos");

            migrationBuilder.DropTable(
                name: "bajas");

            migrationBuilder.DropTable(
                name: "caracteristicas_del_trabjador");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "historicos_de_puestos_de_trabajos");

            migrationBuilder.DropTable(
                name: "OtrosMovimientos");

            migrationBuilder.DropTable(
                name: "Plantilla");

            migrationBuilder.DropTable(
                name: "Traslados");

            migrationBuilder.DropTable(
                name: "actividades_laborales");

            migrationBuilder.DropTable(
                name: "contratos");

            migrationBuilder.DropTable(
                name: "trabajadores");

            migrationBuilder.DropTable(
                name: "municipios");

            migrationBuilder.DropTable(
                name: "puestos_de_trabajos");

            migrationBuilder.DropTable(
                name: "provincias");

            migrationBuilder.DropTable(
                name: "cargo");

            migrationBuilder.DropTable(
                name: "unidades_organizativas");

            migrationBuilder.DropTable(
                name: "grupos_escalas");

            migrationBuilder.DropTable(
                name: "tipos_de_unidad_organizativa");

            migrationBuilder.DropTable(
                name: "categorias_ocupacionales");
        }
    }
}
