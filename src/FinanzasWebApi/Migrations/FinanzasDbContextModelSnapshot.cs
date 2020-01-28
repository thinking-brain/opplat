﻿// <auto-generated />
using System;
using FinanzasWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FinanzasWebApi.Migrations
{
    [DbContext(typeof(FinanzasDbContext))]
    partial class FinanzasDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("FinanzasWebApi.Models.CacheCuentaPeriodo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Acumulado");

                    b.Property<string>("Cuenta");

                    b.Property<DateTime>("FechaActualizado");

                    b.Property<int>("Mes");

                    b.Property<decimal>("Saldo");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("CachesCuentasEnPeriodos");
                });

            modelBuilder.Entity("FinanzasWebApi.Models.CacheEstadoFinanciero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Apertura");

                    b.Property<string>("Concepto");

                    b.Property<string>("EFE");

                    b.Property<DateTime>("FechaActualizado");

                    b.Property<string>("Grupo");

                    b.Property<int>("Mes");

                    b.Property<decimal>("PlanAnual");

                    b.Property<decimal>("Saldo");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("CachesEstadosFinancieros");
                });

            modelBuilder.Entity("FinanzasWebApi.Models.ConfiguracionFirmas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Nombre");

                    b.Property<string>("Valor");

                    b.HasKey("Id");

                    b.ToTable("ConfiguracionesFirmas");
                });

            modelBuilder.Entity("FinanzasWebApi.Models.ConfiguracionPorciento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Porciento");

                    b.Property<string>("Titulo");

                    b.HasKey("Id");

                    b.ToTable("ConfiguracionesPorcientos");
                });
#pragma warning restore 612, 618
        }
    }
}
