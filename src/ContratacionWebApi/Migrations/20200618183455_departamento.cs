using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class departamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminContratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminContratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContratoId_DictaminadorId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    DictaminadorContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoId_DictaminadorId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: false),
                    Direccion = table.Column<string>(nullable: false),
                    Nit = table.Column<string>(nullable: false),
                    CI = table.Column<string>(nullable: true),
                    CarnetTCP = table.Column<string>(nullable: true),
                    Sector = table.Column<int>(nullable: false),
                    Fax = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    ObjetoSocial = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiempoVenContratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoTiempo = table.Column<int>(nullable: false),
                    ContratosProxVencerDesde = table.Column<int>(nullable: false),
                    ContratosProxVencerHasta = table.Column<int>(nullable: false),
                    ContratosCasiVencDesde = table.Column<int>(nullable: false),
                    ContratosCasiVencHasta = table.Column<int>(nullable: false),
                    ContratosVencidos = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiempoVenContratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiempoVenOfertas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfertaTiempo = table.Column<int>(nullable: false),
                    OfertasProxVencDesde = table.Column<int>(nullable: false),
                    OfertasProxVencHasta = table.Column<int>(nullable: false),
                    OfertasCasiVencDesde = table.Column<int>(nullable: false),
                    OfertasCasiVencHasta = table.Column<int>(nullable: false),
                    OfertasVencidas = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiempoVenOfertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DictaminadoresContratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DictaminadorId = table.Column<int>(nullable: false),
                    DepartamentoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictaminadoresContratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictaminadoresContratos_Departamento_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Tipo = table.Column<int>(nullable: false),
                    AdminContratoId = table.Column<int>(nullable: false),
                    EntidadId = table.Column<int>(nullable: false),
                    ObjetoDeContrato = table.Column<string>(nullable: true),
                    Numero = table.Column<string>(nullable: true),
                    MontoCup = table.Column<decimal>(nullable: true),
                    MontoCuc = table.Column<decimal>(nullable: true),
                    MontoUsd = table.Column<decimal>(nullable: true),
                    FechaDeRecepcion = table.Column<DateTime>(nullable: false),
                    FechaDeFirmado = table.Column<DateTime>(nullable: false),
                    FechaVenContrato = table.Column<DateTime>(nullable: false),
                    FechaDeVenOferta = table.Column<DateTime>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    TerminoDePago = table.Column<int>(nullable: false),
                    AprobEconomico = table.Column<bool>(nullable: false),
                    AprobJuridico = table.Column<bool>(nullable: false),
                    AprobComitContratacion = table.Column<bool>(nullable: false),
                    Estado = table.Column<int>(nullable: false),
                    Cliente = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratos_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroCuenta = table.Column<string>(nullable: true),
                    NumeroSucursal = table.Column<string>(nullable: true),
                    NombreSucursal = table.Column<int>(nullable: false),
                    Moneda = table.Column<int>(nullable: false),
                    EntidadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasBancarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EspecialistasExternos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    Apellidos = table.Column<string>(nullable: false),
                    EntidadId = table.Column<int>(nullable: false),
                    Area = table.Column<string>(nullable: true),
                    Departamento = table.Column<string>(nullable: true),
                    Cargo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialistasExternos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EspecialistasExternos_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Telefonos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    EntidadId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefonos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telefonos_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContratoId_FormaPagoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    FormaDePagoId = table.Column<int>(nullable: false),
                    FormaDePago = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoId_FormaPagoId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratoId_FormaPagoId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(nullable: true),
                    NoOficial = table.Column<string>(nullable: true),
                    FechaFirmado = table.Column<DateTime>(nullable: true),
                    Dictamen = table.Column<string>(nullable: true),
                    MontoCup = table.Column<decimal>(nullable: true),
                    MontoCuc = table.Column<decimal>(nullable: true),
                    FechaVenContrato = table.Column<DateTime>(nullable: true),
                    AdminContratoId = table.Column<int>(nullable: false),
                    RevisionActual = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_AdminContratos_AdminContratoId",
                        column: x => x.AdminContratoId,
                        principalTable: "AdminContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documentos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosEstadoContratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    Estado = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Usuario = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosEstadoContratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricosEstadoContratos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EspecialistaExternoId_ContratoId",
                columns: table => new
                {
                    EspecialistaExternoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialistaExternoId_ContratoId", x => new { x.ContratoId, x.EspecialistaExternoId });
                    table.ForeignKey(
                        name: "FK_EspecialistaExternoId_ContratoId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspecialistaExternoId_ContratoId_EspecialistasExternos_Espe~",
                        column: x => x.EspecialistaExternoId,
                        principalTable: "EspecialistasExternos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cont_especialidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    DocumentoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cont_especialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cont_especialidades_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cont_objs_de_contratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    DocumentoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cont_objs_de_contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cont_objs_de_contratos_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoDeDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentoId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Detalles = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoDeDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoDeDocumento_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Especialista",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrabajadorId = table.Column<int>(nullable: false),
                    EspecialidadId = table.Column<int>(nullable: false),
                    DetallesEspecialista = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Especialista_cont_especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "cont_especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dictamen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroDeDictamen = table.Column<string>(nullable: true),
                    EspecialistaId = table.Column<int>(nullable: false),
                    Aprobado = table.Column<bool>(nullable: false),
                    Observaciones = table.Column<string>(nullable: true),
                    FundamentosDeDerecho = table.Column<string>(nullable: false),
                    Consideraciones = table.Column<string>(nullable: true),
                    Recomendaciones = table.Column<string>(nullable: true),
                    OtrosSi = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictamen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dictamen_Especialista_EspecialistaId",
                        column: x => x.EspecialistaId,
                        principalTable: "Especialista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cont_especialidades_DocumentoId",
                table: "cont_especialidades",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_cont_objs_de_contratos_DocumentoId",
                table: "cont_objs_de_contratos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_FormaPagoId_ContratoId",
                table: "ContratoId_FormaPagoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_EntidadId",
                table: "Contratos",
                column: "EntidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamen_EspecialistaId",
                table: "Dictamen",
                column: "EspecialistaId");

            migrationBuilder.CreateIndex(
                name: "IX_DictaminadoresContratos_DepartamentoId",
                table: "DictaminadoresContratos",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_AdminContratoId",
                table: "Documentos",
                column: "AdminContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ContratoId",
                table: "Documentos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Especialista_EspecialidadId",
                table: "Especialista",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_EspecialistaExternoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "EspecialistaExternoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistasExternos_EntidadId",
                table: "EspecialistasExternos",
                column: "EntidadId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoDeDocumento_DocumentoId",
                table: "HistoricoDeDocumento",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosEstadoContratos_ContratoId",
                table: "HistoricosEstadoContratos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Telefonos_EntidadId",
                table: "Telefonos",
                column: "EntidadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cont_objs_de_contratos");

            migrationBuilder.DropTable(
                name: "ContratoId_DictaminadorId");

            migrationBuilder.DropTable(
                name: "ContratoId_FormaPagoId");

            migrationBuilder.DropTable(
                name: "CuentasBancarias");

            migrationBuilder.DropTable(
                name: "Dictamen");

            migrationBuilder.DropTable(
                name: "DictaminadoresContratos");

            migrationBuilder.DropTable(
                name: "EspecialistaExternoId_ContratoId");

            migrationBuilder.DropTable(
                name: "HistoricoDeDocumento");

            migrationBuilder.DropTable(
                name: "HistoricosEstadoContratos");

            migrationBuilder.DropTable(
                name: "Telefonos");

            migrationBuilder.DropTable(
                name: "TiempoVenContratos");

            migrationBuilder.DropTable(
                name: "TiempoVenOfertas");

            migrationBuilder.DropTable(
                name: "Especialista");

            migrationBuilder.DropTable(
                name: "Departamento");

            migrationBuilder.DropTable(
                name: "EspecialistasExternos");

            migrationBuilder.DropTable(
                name: "cont_especialidades");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "AdminContratos");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Entidades");
        }
    }
}
