using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class iniciar_datos_en_1_sin_definir : Migration
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
                name: "ContratoTrabs",
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
                    table.PrimaryKey("PK_ContratoTrabs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilOcupacional",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilOcupacional", x => x.Id);
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
                name: "actividades_de_ContratoTrabs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ActividadLaboralId = table.Column<int>(nullable: false),
                    ContratoTrabId = table.Column<int>(nullable: false),
                    PrecioCUC = table.Column<decimal>(nullable: false),
                    PrecioCUP = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades_de_ContratoTrabs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_actividades_de_ContratoTrabs_actividades_laborales_Activida~",
                        column: x => x.ActividadLaboralId,
                        principalTable: "actividades_laborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_actividades_de_ContratoTrabs_ContratoTrabs_ContratoTrabId",
                        column: x => x.ContratoTrabId,
                        principalTable: "ContratoTrabs",
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
                    TipoUnidadOrganizativaId = table.Column<int>(nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cargo",
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
                    table.PrimaryKey("PK_Cargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cargo_grupos_escalas_GrupoEscalaId",
                        column: x => x.GrupoEscalaId,
                        principalTable: "grupos_escalas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cargo_Cargo_JefeId",
                        column: x => x.JefeId,
                        principalTable: "Cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "caracteristicas_de_los_socios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Direccion = table.Column<string>(nullable: true),
                    Sexo = table.Column<int>(nullable: true),
                    MunicipioId = table.Column<int>(nullable: true),
                    PerfilOcupacionalId = table.Column<int>(nullable: true),
                    NivelDeEscolaridad = table.Column<int>(nullable: false),
                    EdadDesde = table.Column<int>(nullable: true),
                    EdadHasta = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caracteristicas_de_los_socios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_caracteristicas_de_los_socios_municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_caracteristicas_de_los_socios_PerfilOcupacional_PerfilOcupa~",
                        column: x => x.PerfilOcupacionalId,
                        principalTable: "PerfilOcupacional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    CargoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funciones_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
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
                        name: "FK_Plantilla_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
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
                        name: "FK_puestos_de_trabajos_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
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
                name: "Requisitos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    CargoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requisitos_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AperturaSocio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    CantTrabajadores = table.Column<int>(nullable: false),
                    NumeroAcuerdo = table.Column<int>(nullable: false),
                    CaracteristicasSocioId = table.Column<int>(nullable: true),
                    EstadosApertura = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AperturaSocio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AperturaSocio_caracteristicas_de_los_socios_Caracteristicas~",
                        column: x => x.CaracteristicasSocioId,
                        principalTable: "caracteristicas_de_los_socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trabajadores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: false),
                    Apellidos = table.Column<string>(nullable: false),
                    CI = table.Column<string>(maxLength: 11, nullable: false),
                    TelefonoFijo = table.Column<string>(nullable: true),
                    TelefonoMovil = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    Sexo = table.Column<int>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    MunicipioId = table.Column<int>(nullable: true),
                    PerfilOcupacionalId = table.Column<int>(nullable: false),
                    PuestoDeTrabajoId = table.Column<int>(nullable: true),
                    NivelDeEscolaridad = table.Column<int>(nullable: false),
                    EstadoTrabajador = table.Column<int>(nullable: false),
                    AperturaSocioId = table.Column<int>(nullable: true),
                    Fecha_Nac = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trabajadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trabajadores_AperturaSocio_AperturaSocioId",
                        column: x => x.AperturaSocioId,
                        principalTable: "AperturaSocio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajadores_municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                        column: x => x.PerfilOcupacionalId,
                        principalTable: "PerfilOcupacional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Bolsa",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrabajadorId = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Nombre_Referencia = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolsa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bolsa_trabajadores_TrabajadorId",
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
                    TallaCalzado = table.Column<double>(nullable: true),
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
                        name: "FK_Entradas_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
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
                    EstadoOrden = table.Column<int>(nullable: false),
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
                    UnidOrgOrigenId = table.Column<int>(nullable: false),
                    UnidOrgDestinoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traslados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traslados_Cargo_CargoDestinoId",
                        column: x => x.CargoDestinoId,
                        principalTable: "Cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traslados_Cargo_CargoOrigenId",
                        column: x => x.CargoOrigenId,
                        principalTable: "Cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Traslados_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traslados_unidades_organizativas_UnidOrgDestinoId",
                        column: x => x.UnidOrgDestinoId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traslados_unidades_organizativas_UnidOrgOrigenId",
                        column: x => x.UnidOrgOrigenId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PerfilOcupacional",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Sin Definir" });

            migrationBuilder.InsertData(
                table: "categorias_ocupacionales",
                columns: new[] { "Id", "Descripcion" },
                values: new object[] { 1, "Sin Definir" });

            migrationBuilder.InsertData(
                table: "provincias",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Sin Definir" });

            migrationBuilder.InsertData(
                table: "tipos_de_unidad_organizativa",
                columns: new[] { "Id", "Nombre", "Prioridad" },
                values: new object[] { 1, "Sin Definir", 0 });

            migrationBuilder.InsertData(
                table: "grupos_escalas",
                columns: new[] { "Id", "CategoriaOcupacionalId", "Codigo", "SalarioDiferenciado", "SalarioEscala" },
                values: new object[] { 1, 1, "Sin Definir", false, 0m });

            migrationBuilder.InsertData(
                table: "municipios",
                columns: new[] { "Id", "Nombre", "ProvinciaId" },
                values: new object[] { 1, "Sin Definir", 1 });

            migrationBuilder.InsertData(
                table: "unidades_organizativas",
                columns: new[] { "Id", "Activa", "Codigo", "Nombre", "PerteneceAId", "TipoUnidadOrganizativaId" },
                values: new object[] { 1, false, "Sin Definir", "Sin Definir", null, 1 });

            migrationBuilder.InsertData(
                table: "Cargo",
                columns: new[] { "Id", "GrupoEscalaId", "JefeId", "Nombre", "Sigla" },
                values: new object[] { 1, 1, null, "Sin Definir", "Sin Definir" });

            migrationBuilder.InsertData(
                table: "puestos_de_trabajos",
                columns: new[] { "Id", "CargoId", "Descripcion", "JefeId", "PlantillaOcupada", "UnidadOrganizativaId" },
                values: new object[] { 1, 1, "Sin Definir", null, 0, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_de_ContratoTrabs_ActividadLaboralId",
                table: "actividades_de_ContratoTrabs",
                column: "ActividadLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_actividades_de_ContratoTrabs_ContratoTrabId",
                table: "actividades_de_ContratoTrabs",
                column: "ContratoTrabId");

            migrationBuilder.CreateIndex(
                name: "IX_AperturaSocio_CaracteristicasSocioId",
                table: "AperturaSocio",
                column: "CaracteristicasSocioId");

            migrationBuilder.CreateIndex(
                name: "IX_bajas_TrabajadorId",
                table: "bajas",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bolsa_TrabajadorId",
                table: "Bolsa",
                column: "TrabajadorId");

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_de_los_socios_MunicipioId",
                table: "caracteristicas_de_los_socios",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_de_los_socios_PerfilOcupacionalId",
                table: "caracteristicas_de_los_socios",
                column: "PerfilOcupacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_del_trabjador_TrabajadorId",
                table: "caracteristicas_del_trabjador",
                column: "TrabajadorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_GrupoEscalaId",
                table: "Cargo",
                column: "GrupoEscalaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_JefeId",
                table: "Cargo",
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
                name: "IX_Funciones_CargoId",
                table: "Funciones",
                column: "CargoId");

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
                name: "IX_Requisitos_CargoId",
                table: "Requisitos",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_AperturaSocioId",
                table: "trabajadores",
                column: "AperturaSocioId");

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_MunicipioId",
                table: "trabajadores",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_PerfilOcupacionalId",
                table: "trabajadores",
                column: "PerfilOcupacionalId");

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
                name: "IX_Traslados_UnidOrgDestinoId",
                table: "Traslados",
                column: "UnidOrgDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_UnidOrgOrigenId",
                table: "Traslados",
                column: "UnidOrgOrigenId");

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
                name: "actividades_de_ContratoTrabs");

            migrationBuilder.DropTable(
                name: "bajas");

            migrationBuilder.DropTable(
                name: "Bolsa");

            migrationBuilder.DropTable(
                name: "caracteristicas_del_trabjador");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "Funciones");

            migrationBuilder.DropTable(
                name: "historicos_de_puestos_de_trabajos");

            migrationBuilder.DropTable(
                name: "OtrosMovimientos");

            migrationBuilder.DropTable(
                name: "Plantilla");

            migrationBuilder.DropTable(
                name: "Requisitos");

            migrationBuilder.DropTable(
                name: "Traslados");

            migrationBuilder.DropTable(
                name: "actividades_laborales");

            migrationBuilder.DropTable(
                name: "ContratoTrabs");

            migrationBuilder.DropTable(
                name: "trabajadores");

            migrationBuilder.DropTable(
                name: "AperturaSocio");

            migrationBuilder.DropTable(
                name: "puestos_de_trabajos");

            migrationBuilder.DropTable(
                name: "caracteristicas_de_los_socios");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "unidades_organizativas");

            migrationBuilder.DropTable(
                name: "municipios");

            migrationBuilder.DropTable(
                name: "PerfilOcupacional");

            migrationBuilder.DropTable(
                name: "grupos_escalas");

            migrationBuilder.DropTable(
                name: "tipos_de_unidad_organizativa");

            migrationBuilder.DropTable(
                name: "provincias");

            migrationBuilder.DropTable(
                name: "categorias_ocupacionales");
        }
    }
}
