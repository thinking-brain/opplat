﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using op_finanzas_api.Models;

namespace op_finanzas_api.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20190829182612_migUpdPlanPP")]
    partial class migUpdPlanPP
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("op_finanzas_api.Models.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo");

                    b.Property<string>("Nombre");

                    b.HasKey("Id");

                    b.ToTable("Costos_Area");
                });

            modelBuilder.Entity("op_finanzas_api.Models.CentroCostoArea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId");

                    b.Property<string>("CentroCostoId");

                    b.Property<string>("Detalles");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Costos_CentroCostoArea");
                });

            modelBuilder.Entity("op_finanzas_api.Models.ExportExcel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Objeto");

                    b.HasKey("Id");

                    b.ToTable("Costos_ExportExcel");
                });

            modelBuilder.Entity("op_finanzas_api.Models.GrupoSubelemento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo");

                    b.Property<string>("Nombre");

                    b.HasKey("Id");

                    b.ToTable("Costos_GrupoSubelemento");
                });

            modelBuilder.Entity("op_finanzas_api.Models.GrupoSubElemento_SubElemento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion");

                    b.Property<int>("GrupoSubelementoId");

                    b.Property<string>("SubElementoGastoId");

                    b.HasKey("Id");

                    b.HasIndex("GrupoSubelementoId");

                    b.ToTable("Costos_GrupoSubElemento_SubElemento");
                });

            modelBuilder.Entity("op_finanzas_api.Models.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Abril");

                    b.Property<decimal>("Agosto");

                    b.Property<string>("Analisis");

                    b.Property<string>("CentroCosto");

                    b.Property<int>("CentroCostoAreaId");

                    b.Property<string>("Cuenta");

                    b.Property<string>("Descripcion");

                    b.Property<decimal>("Diciembre");

                    b.Property<string>("Elemento");

                    b.Property<decimal>("Enero");

                    b.Property<decimal>("Febrero");

                    b.Property<DateTime>("Fecha");

                    b.Property<decimal>("Julio");

                    b.Property<decimal>("Junio");

                    b.Property<decimal>("Marzo");

                    b.Property<decimal>("Mayo");

                    b.Property<decimal>("Noviembre");

                    b.Property<decimal>("Octubre");

                    b.Property<decimal>("Septiembre");

                    b.Property<string>("SubAnalisis");

                    b.Property<string>("SubCuenta");

                    b.Property<string>("SubElemento");

                    b.Property<string>("SubElementoTope");

                    b.HasKey("Id");

                    b.ToTable("Costos_Plan");
                });

            modelBuilder.Entity("op_finanzas_api.Models.PlanGI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Abril");

                    b.Property<decimal>("Agosto");

                    b.Property<string>("Año");

                    b.Property<decimal>("Diciembre");

                    b.Property<string>("Elemento");

                    b.Property<string>("ElementoValor");

                    b.Property<decimal>("Enero");

                    b.Property<decimal>("Febrero");

                    b.Property<DateTime>("Fecha");

                    b.Property<decimal>("Julio");

                    b.Property<decimal>("Junio");

                    b.Property<decimal>("Marzo");

                    b.Property<decimal>("Mayo");

                    b.Property<decimal>("Noviembre");

                    b.Property<decimal>("Octubre");

                    b.Property<decimal>("Septiembre");

                    b.HasKey("Id");

                    b.ToTable("Costos_PlanGI");
                });

            modelBuilder.Entity("op_finanzas_api.Models.PlanPronosticoProductivo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Abril");

                    b.Property<decimal>("Agosto");

                    b.Property<string>("Año");

                    b.Property<string>("CuentaCUC");

                    b.Property<string>("CuentaMN");

                    b.Property<decimal>("Diciembre");

                    b.Property<decimal>("Enero");

                    b.Property<decimal>("Febrero");

                    b.Property<DateTime>("Fecha");

                    b.Property<decimal>("Julio");

                    b.Property<decimal>("Junio");

                    b.Property<decimal>("Marzo");

                    b.Property<decimal>("Mayo");

                    b.Property<decimal>("Noviembre");

                    b.Property<decimal>("Octubre");

                    b.Property<decimal>("Septiembre");

                    b.HasKey("Id");

                    b.ToTable("Costos_PlanPronosticoProductivo");
                });

            modelBuilder.Entity("op_finanzas_api.Models.SubMayor", b =>
                {
                    b.Property<string>("Analisis")
                        .HasMaxLength(20);

                    b.Property<short>("Ano");

                    b.Property<string>("Cta")
                        .HasMaxLength(20);

                    b.Property<decimal>("Debe")
                        .HasColumnType("money");

                    b.Property<string>("Epigrafe")
                        .HasMaxLength(20);

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("smalldatetime");

                    b.Property<decimal>("Haber")
                        .HasColumnType("money");

                    b.Property<byte>("Mes");

                    b.Property<string>("SubAnalisis")
                        .HasMaxLength(20);

                    b.Property<string>("SubCta")
                        .HasMaxLength(20);

                    b.HasKey("Analisis", "Ano", "Cta", "Debe", "Epigrafe", "Fecha", "Haber", "Mes", "SubAnalisis", "SubCta");

                    b.ToTable("Costos_Submayor");
                });

            modelBuilder.Entity("op_finanzas_api.Models.SubMayorCuenta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Año");

                    b.Property<string>("ClaveCuenta");

                    b.Property<int>("IdCuenta");

                    b.Property<decimal>("Importe");

                    b.Property<int>("Mes");

                    b.HasKey("Id");

                    b.ToTable("Costos_SubmayorDeCuentas");
                });

            modelBuilder.Entity("op_finanzas_api.Models.CentroCostoArea", b =>
                {
                    b.HasOne("op_finanzas_api.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("op_finanzas_api.Models.GrupoSubElemento_SubElemento", b =>
                {
                    b.HasOne("op_finanzas_api.Models.GrupoSubelemento", "GrupoSubelemento")
                        .WithMany()
                        .HasForeignKey("GrupoSubelementoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
