using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class ModelsFaltante1s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminContratos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrabajadorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminContratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DictaminadoresContrato",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrabajadorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictaminadoresContrato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    CodigoReup = table.Column<string>(nullable: false),
                    Direccion = table.Column<string>(nullable: false),
                    Nit = table.Column<string>(nullable: false),
                    CtaBancariaCuc = table.Column<string>(nullable: true),
                    CtaBancariaMn = table.Column<string>(nullable: true),
                    NombreCtaCuc = table.Column<string>(nullable: true),
                    NombreCtaMn = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EspecialistasExternos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    Cargo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialistasExternos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormasDePagos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasDePagos", x => x.Id);
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
                    FechaDeLlegada = table.Column<DateTime>(nullable: false),
                    FechaDeFirmado = table.Column<DateTime>(nullable: true),
                    FechaDeVencimiento = table.Column<DateTime>(nullable: true),
                    TerminoDePago = table.Column<int>(nullable: false),
                    AprobEconomico = table.Column<bool>(nullable: false),
                    AprobJuridico = table.Column<bool>(nullable: false),
                    AprobComitContratacion = table.Column<bool>(nullable: false),
                    Estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratos_AdminContratos_AdminContratoId",
                        column: x => x.AdminContratoId,
                        principalTable: "AdminContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contratos_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_ContratoId_DictaminadorId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_Dictaminad~",
                        column: x => x.DictaminadorContratoId,
                        principalTable: "DictaminadoresContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContratoId_FormaPagoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    FormaDePagoId = table.Column<int>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ContratoId_FormaPagoId_FormasDePagos_FormaDePagoId",
                        column: x => x.FormaDePagoId,
                        principalTable: "FormasDePagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoDeContrato",
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
                    FechaDeVencimiento = table.Column<DateTime>(nullable: true),
                    AdminContratoId = table.Column<int>(nullable: false),
                    RevisionActual = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoDeContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentoDeContrato_AdminContratos_AdminContratoId",
                        column: x => x.AdminContratoId,
                        principalTable: "AdminContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentoDeContrato_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EspecialistaExternoId_ContratoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EspecialistaExternoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false),
                    Estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialistaExternoId_ContratoId", x => x.Id);
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
                name: "cont_especialidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: false),
                    DocumentoDeContratoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cont_especialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cont_especialidades_DocumentoDeContrato_DocumentoDeContrato~",
                        column: x => x.DocumentoDeContratoId,
                        principalTable: "DocumentoDeContrato",
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
                    DocumentoDeContratoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cont_objs_de_contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cont_objs_de_contratos_DocumentoDeContrato_DocumentoDeContr~",
                        column: x => x.DocumentoDeContratoId,
                        principalTable: "DocumentoDeContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoDeDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentoDeContratoId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Detalles = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoDeDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoDeDocumento_DocumentoDeContrato_DocumentoDeContrat~",
                        column: x => x.DocumentoDeContratoId,
                        principalTable: "DocumentoDeContrato",
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
                name: "IX_cont_especialidades_DocumentoDeContratoId",
                table: "cont_especialidades",
                column: "DocumentoDeContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_cont_objs_de_contratos_DocumentoDeContratoId",
                table: "cont_objs_de_contratos",
                column: "DocumentoDeContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_ContratoId",
                table: "ContratoId_DictaminadorId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_DictaminadorContratoId",
                table: "ContratoId_DictaminadorId",
                column: "DictaminadorContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_FormaPagoId_ContratoId",
                table: "ContratoId_FormaPagoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_FormaPagoId_FormaDePagoId",
                table: "ContratoId_FormaPagoId",
                column: "FormaDePagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_AdminContratoId",
                table: "Contratos",
                column: "AdminContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_EntidadId",
                table: "Contratos",
                column: "EntidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamen_EspecialistaId",
                table: "Dictamen",
                column: "EspecialistaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoDeContrato_AdminContratoId",
                table: "DocumentoDeContrato",
                column: "AdminContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoDeContrato_ContratoId",
                table: "DocumentoDeContrato",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Especialista_EspecialidadId",
                table: "Especialista",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_EspecialistaExternoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "EspecialistaExternoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoDeDocumento_DocumentoDeContratoId",
                table: "HistoricoDeDocumento",
                column: "DocumentoDeContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosEstadoContratos_ContratoId",
                table: "HistoricosEstadoContratos",
                column: "ContratoId");
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
                name: "Dictamen");

            migrationBuilder.DropTable(
                name: "EspecialistaExternoId_ContratoId");

            migrationBuilder.DropTable(
                name: "HistoricoDeDocumento");

            migrationBuilder.DropTable(
                name: "HistoricosEstadoContratos");

            migrationBuilder.DropTable(
                name: "DictaminadoresContrato");

            migrationBuilder.DropTable(
                name: "FormasDePagos");

            migrationBuilder.DropTable(
                name: "Especialista");

            migrationBuilder.DropTable(
                name: "EspecialistasExternos");

            migrationBuilder.DropTable(
                name: "cont_especialidades");

            migrationBuilder.DropTable(
                name: "DocumentoDeContrato");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "AdminContratos");

            migrationBuilder.DropTable(
                name: "Entidades");
        }
    }
}
