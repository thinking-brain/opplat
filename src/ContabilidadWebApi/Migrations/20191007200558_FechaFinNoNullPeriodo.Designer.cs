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
    [Migration("20191007200558_FechaFinNoNullPeriodo")]
    partial class FechaFinNoNullPeriodo
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

            modelBuilder.Entity("ContabilidadWebApi.Models.Asiento", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.DiaContable", "DiaContable")
                        .WithMany("Asientos")
                        .HasForeignKey("DiaContableId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContabilidadWebApi.Models.Cuenta", b =>
                {
                    b.HasOne("ContabilidadWebApi.Models.Cuenta", "CuentaSuperior")
                        .WithMany("Subcuentas")
                        .HasForeignKey("CuentaSuperiorId")
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
#pragma warning restore 612, 618
        }
    }
}
