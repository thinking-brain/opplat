﻿// <auto-generated />
using System;
using ContratacionWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    [DbContext(typeof(ContratacionDbContext))]
    [Migration("20200423134858_VencimientoOferta")]
    partial class VencimientoOferta
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ContratacionWebApi.Models.AdminContrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("TrabajadorId");

                    b.HasKey("Id");

                    b.ToTable("AdminContratos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Contrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AprobComitContratacion");

                    b.Property<bool>("AprobEconomico");

                    b.Property<bool>("AprobJuridico");

                    b.Property<int>("EntidadId");

                    b.Property<int>("Estado");

                    b.Property<DateTime?>("FechaDeFirmado");

                    b.Property<DateTime>("FechaDeRecepcion");

                    b.Property<DateTime>("FechaDeVenOferta");

                    b.Property<DateTime>("FechaVenContrato");

                    b.Property<string>("FilePath");

                    b.Property<decimal?>("MontoCuc");

                    b.Property<decimal?>("MontoCup");

                    b.Property<string>("Nombre");

                    b.Property<string>("Numero");

                    b.Property<string>("ObjetoDeContrato");

                    b.Property<double>("TerminoDePago");

                    b.Property<int>("Tipo");

                    b.Property<int>("TrabajadorId");

                    b.Property<int>("Vigencia");

                    b.HasKey("Id");

                    b.HasIndex("EntidadId");

                    b.HasIndex("TrabajadorId");

                    b.ToTable("Contratos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ContratoId_DictaminadorId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContratoId");

                    b.Property<int>("DictaminadorContratoId");

                    b.Property<int?>("DictaminadorId");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("DictaminadorId");

                    b.ToTable("ContratoId_DictaminadorId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ContratoId_FormaPagoId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContratoId");

                    b.Property<int>("FormaDePago");

                    b.Property<int>("FormaDePagoId");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.ToTable("ContratoId_FormaPagoId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.CuentaBancaria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EntidadId");

                    b.Property<int>("Moneda");

                    b.Property<int>("NombreSucursal");

                    b.Property<string>("NumeroCuenta");

                    b.Property<string>("NumeroSucursal");

                    b.HasKey("Id");

                    b.HasIndex("EntidadId");

                    b.ToTable("CuentasBancarias");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Dictamen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Aprobado");

                    b.Property<string>("Consideraciones");

                    b.Property<int>("EspecialistaId");

                    b.Property<string>("FundamentosDeDerecho")
                        .IsRequired();

                    b.Property<string>("NumeroDeDictamen");

                    b.Property<string>("Observaciones");

                    b.Property<string>("OtrosSi");

                    b.Property<string>("Recomendaciones");

                    b.HasKey("Id");

                    b.HasIndex("EspecialistaId");

                    b.ToTable("Dictamen");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Dictaminador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("TrabajadorId");

                    b.HasKey("Id");

                    b.ToTable("Dictaminadores");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Documento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdminContratoId");

                    b.Property<string>("Dictamen");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime?>("FechaVenContrato");

                    b.Property<DateTime?>("FechaFirmado");

                    b.Property<decimal?>("MontoCuc");

                    b.Property<decimal?>("MontoCup");

                    b.Property<string>("NoOficial");

                    b.Property<string>("Numero");

                    b.Property<string>("RevisionActual");

                    b.HasKey("Id");

                    b.HasIndex("AdminContratoId");

                    b.ToTable("Documentos");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Documento");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Entidad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Correo");

                    b.Property<string>("Direccion")
                        .IsRequired();

                    b.Property<string>("Fax");

                    b.Property<string>("Nit")
                        .IsRequired();

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.Property<string>("ObjetoSocial");

                    b.Property<int>("Sector");

                    b.HasKey("Id");

                    b.ToTable("Entidades");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.EspExternoId_ContratoId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContratoId");

                    b.Property<int>("EspecialistaExternoId");

                    b.Property<int>("Estado");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("EspecialistaExternoId");

                    b.ToTable("EspecialistaExternoId_ContratoId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Especialidad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DocumentoId");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DocumentoId");

                    b.ToTable("cont_especialidades");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Especialista", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("DetallesEspecialista");

                    b.Property<int>("EspecialidadId");

                    b.Property<int>("TrabajadorId");

                    b.HasKey("Id");

                    b.HasIndex("EspecialidadId");

                    b.ToTable("Especialista");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.EspecialistaExterno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Apellidos")
                        .IsRequired();

                    b.Property<string>("Area");

                    b.Property<string>("Cargo");

                    b.Property<string>("Departamento");

                    b.Property<int>("EntidadId");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("EntidadId");

                    b.ToTable("EspecialistasExternos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.HistoricoDeDocumento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Detalles");

                    b.Property<int>("DocumentoId");

                    b.Property<DateTime>("Fecha");

                    b.Property<int>("UsuarioId");

                    b.HasKey("Id");

                    b.HasIndex("DocumentoId");

                    b.ToTable("HistoricoDeDocumento");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.HistoricoEstadoContrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContratoId");

                    b.Property<int>("Estado");

                    b.Property<DateTime>("Fecha");

                    b.Property<string>("Usuario");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.ToTable("HistoricosEstadoContratos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ObjetoDeContrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DocumentoId");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DocumentoId");

                    b.ToTable("cont_objs_de_contratos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Telefono", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EntidadId");

                    b.Property<string>("Extension");

                    b.Property<string>("Numero");

                    b.HasKey("Id");

                    b.HasIndex("EntidadId");

                    b.ToTable("Telefonos");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Suplemento", b =>
                {
                    b.HasBaseType("ContratacionWebApi.Models.Documento");

                    b.Property<int>("ContratoId");

                    b.HasIndex("ContratoId");

                    b.HasDiscriminator().HasValue("Suplemento");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Contrato", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Entidad", "Entidad")
                        .WithMany()
                        .HasForeignKey("EntidadId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContratacionWebApi.Models.AdminContrato", "Trabajador")
                        .WithMany()
                        .HasForeignKey("TrabajadorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ContratoId_DictaminadorId", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContratacionWebApi.Models.Dictaminador", "Dictaminador")
                        .WithMany()
                        .HasForeignKey("DictaminadorId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ContratoId_FormaPagoId", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.CuentaBancaria", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Entidad", "Entidad")
                        .WithMany("CuentasBancarias")
                        .HasForeignKey("EntidadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Dictamen", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Especialista", "Especialista")
                        .WithMany("Dictamenes")
                        .HasForeignKey("EspecialistaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Documento", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.AdminContrato", "AdminContrato")
                        .WithMany()
                        .HasForeignKey("AdminContratoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.EspExternoId_ContratoId", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContratacionWebApi.Models.EspecialistaExterno", "EspecialistaExterno")
                        .WithMany()
                        .HasForeignKey("EspecialistaExternoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Especialidad", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Documento")
                        .WithMany("Especialidades")
                        .HasForeignKey("DocumentoId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Especialista", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Especialidad", "Especialidad")
                        .WithMany("Especialistas")
                        .HasForeignKey("EspecialidadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.EspecialistaExterno", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Entidad", "Entidad")
                        .WithMany()
                        .HasForeignKey("EntidadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.HistoricoDeDocumento", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Documento", "Documento")
                        .WithMany("Historicos")
                        .HasForeignKey("DocumentoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.HistoricoEstadoContrato", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContratacionWebApi.Models.ObjetoDeContrato", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Documento")
                        .WithMany("ObjetosDeContrato")
                        .HasForeignKey("DocumentoId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Telefono", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Entidad", "Entidad")
                        .WithMany("Telefonos")
                        .HasForeignKey("EntidadId");
                });

            modelBuilder.Entity("ContratacionWebApi.Models.Suplemento", b =>
                {
                    b.HasOne("ContratacionWebApi.Models.Contrato", "Contrato")
                        .WithMany("Suplementos")
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
