﻿// <auto-generated />
using System;
using ContabilidadWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    [DbContext(typeof(ContabilidadDbContext))]
    [Migration("20200206150018_AddGastos")]
    partial class AddGastos
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ContabilidadWebApi.Models.Asiento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Detalle");

                    b.Property<int>("DiaContableId");

                    b.Property<DateTime>("Fecha");

                    b.Property<string>("UsuarioId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DiaContableId");

                    b.ToTable("contb_asientos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.ConceptoCuentas", b =>
                {
                    b.Property<int>("ConceptoPlanId");

                    b.Property<int>("CuentaId");

                    b.HasKey("ConceptoPlanId", "CuentaId");

                    b.HasIndex("CuentaId");

                    b.ToTable("ConceptoCuentas");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.ConceptoPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Concepto");

                    b.HasKey("Id");

                    b.ToTable("ConceptoPlan");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Cuenta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CuentaSuperiorId");

                    b.Property<int>("Naturaleza");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.Property<string>("NumeroParcial")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CuentaSuperiorId");

                    b.ToTable("contb_cuentas");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.CuentaElementoDeGasto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CuentaId");

                    b.Property<int>("ElementoId");

                    b.HasKey("Id");

                    b.HasIndex("CuentaId");

                    b.HasIndex("ElementoId");

                    b.ToTable("CuentaElementoDeGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.DetallePlanGI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Abril");

                    b.Property<decimal>("Agosto");

                    b.Property<int>("ConceptoId");

                    b.Property<decimal>("Diciembre");

                    b.Property<decimal>("Enero");

                    b.Property<decimal>("Febrero");

                    b.Property<decimal>("Julio");

                    b.Property<decimal>("Junio");

                    b.Property<decimal>("Marzo");

                    b.Property<decimal>("Mayo");

                    b.Property<decimal>("Noviembre");

                    b.Property<decimal>("Octubre");

                    b.Property<int>("PlanId");

                    b.Property<decimal>("Septiembre");

                    b.HasKey("Id");

                    b.HasIndex("ConceptoId");

                    b.HasIndex("PlanId");

                    b.ToTable("DetallePlanGI");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.DiaContable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Abierto");

                    b.Property<DateTime>("Fecha");

                    b.Property<DateTime?>("HoraEnQueCerro");

                    b.Property<int>("PeriodoContableId");

                    b.HasKey("Id");

                    b.HasIndex("PeriodoContableId");

                    b.ToTable("contb_dia_contable");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.ElementoDeGasto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Codigo");

                    b.Property<string>("Descripcion");

                    b.HasKey("Id");

                    b.ToTable("ElementoDeGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Movimiento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AsientoId");

                    b.Property<int>("CuentaId");

                    b.Property<decimal>("Importe");

                    b.Property<int>("TipoDeOperacion");

                    b.HasKey("Id");

                    b.HasIndex("AsientoId");

                    b.HasIndex("CuentaId");

                    b.ToTable("contb_movimientos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.PartidaDeGasto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Codigo");

                    b.Property<string>("Desripcion");

                    b.HasKey("Id");

                    b.ToTable("PartidaDeGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.PeriodoContable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<DateTime>("FechaFin");

                    b.Property<DateTime>("FechaInicio");

                    b.HasKey("Id");

                    b.ToTable("PeriodoContable");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.PlanGI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Fecha");

                    b.Property<string>("Titulo");

                    b.Property<string>("Year");

                    b.HasKey("Id");

                    b.ToTable("PlanesIngresosGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.RegistroDeGasto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AsientoId");

                    b.Property<int>("SubElementoId");

                    b.HasKey("Id");

                    b.HasIndex("AsientoId");

                    b.HasIndex("SubElementoId");

                    b.ToTable("RegistroDeGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.SubElementoDeGasto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Codigo");

                    b.Property<string>("Descripcion");

                    b.Property<int>("ElementoId");

                    b.Property<bool>("MonedaNacional");

                    b.Property<int>("PartidaId");

                    b.HasKey("Id");

                    b.HasIndex("ElementoId");

                    b.HasIndex("PartidaId");

                    b.ToTable("SubElementoDeGastos");
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Asiento", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.DiaContable", "DiaContable")
                        .WithMany("Asientos")
                        .HasForeignKey("DiaContableId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.ConceptoCuentas", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.ConceptoPlan", "ConceptoPlan")
                        .WithMany("Cuentas")
                        .HasForeignKey("ConceptoPlanId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.Cuenta", "Cuenta")
                        .WithMany()
                        .HasForeignKey("CuentaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Cuenta", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.Cuenta", "CuentaSuperior")
                        .WithMany("Subcuentas")
                        .HasForeignKey("CuentaSuperiorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.CuentaElementoDeGasto", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.Cuenta", "Cuenta")
                        .WithMany()
                        .HasForeignKey("CuentaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.ElementoDeGasto", "Elemento")
                        .WithMany()
                        .HasForeignKey("ElementoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.DetallePlanGI", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.ConceptoPlan", "Concepto")
                        .WithMany()
                        .HasForeignKey("ConceptoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.PlanGI", "Plan")
                        .WithMany("Detalles")
                        .HasForeignKey("PlanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.DiaContable", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.PeriodoContable", "PeriodoContable")
                        .WithMany()
                        .HasForeignKey("PeriodoContableId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Movimiento", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.Asiento", "Asiento")
                        .WithMany("Movimientos")
                        .HasForeignKey("AsientoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.Cuenta", "Cuenta")
                        .WithMany("Movimientos")
                        .HasForeignKey("CuentaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.RegistroDeGasto", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.Asiento", "Asiento")
                        .WithMany()
                        .HasForeignKey("AsientoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.SubElementoDeGasto", "SubElemento")
                        .WithMany()
                        .HasForeignKey("SubElementoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.SubElementoDeGasto", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.ElementoDeGasto", "Elemento")
                        .WithMany("Subelementos")
                        .HasForeignKey("ElementoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContabilidadWebApi.Models.PartidaDeGasto", "Partida")
                        .WithMany()
                        .HasForeignKey("PartidaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
