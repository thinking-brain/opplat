using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class adminContratoFix : Migration
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
                    FechaDeFirmado = table.Column<DateTime>(nullable: false),
                    FechaDeVencimiento = table.Column<DateTime>(nullable: false),
                    TerminoDePago = table.Column<int>(nullable: false)
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
                name: "EspecialistaExternoId_ContratoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EspecialistaExternoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false)
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
                name: "IX_EspecialistaExternoId_ContratoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_EspecialistaExternoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "EspecialistaExternoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosEstadoContratos_ContratoId",
                table: "HistoricosEstadoContratos",
                column: "ContratoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratoId_DictaminadorId");

            migrationBuilder.DropTable(
                name: "ContratoId_FormaPagoId");

            migrationBuilder.DropTable(
                name: "EspecialistaExternoId_ContratoId");

            migrationBuilder.DropTable(
                name: "HistoricosEstadoContratos");

            migrationBuilder.DropTable(
                name: "DictaminadoresContrato");

            migrationBuilder.DropTable(
                name: "FormasDePagos");

            migrationBuilder.DropTable(
                name: "EspecialistasExternos");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "AdminContratos");

            migrationBuilder.DropTable(
                name: "Entidades");
        }
    }
}
