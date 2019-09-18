using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace op_contabilidad_api.VersatModels
{
    public class VersatDbContext : DbContext
    {
        public VersatDbContext()
        {
        }

        public VersatDbContext(DbContextOptions<VersatDbContext> options)
            : base(options)
        {
        }
        public DbSet<op_contabilidad_api.VersatModels.GenPeriodo> GenPeriodo { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.ConCuentanat> ConCuentanat { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.CosCentro> CosCentro { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.ConCuenta> ConCuenta { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.CosElementogasto> CosElementogasto { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.CosSubelementogasto> CosSubelementogasto { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.GenUnidadcontable> GenUnidadcontable { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.ConCuentamoneda> ConCuentamoneda { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.GenMoneda> GenMoneda { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.CosCuentasAsociadas> CosCuentasAsociadas { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.CosPartida> CosPartida { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.OptCuentaCentroPeriodo> OptCuentaCentroPeriodo { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.OptCuentaPeriodoOK> OptCuentaPeriodoOK { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomAsiento> NomAsiento { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomAsientoGasto> NomAsientoGasto { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomDocumento> NomDocumento { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomDocumentoDetallePago> NomDocumentoDetallePago { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomPeriodopago> NomPeriodopago { get; set; }
        public DbSet<op_contabilidad_api.VersatModels.NomTipoDocumento> NomTipoDocumento { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<CosPartida>(entity =>
            {
                entity.HasKey(e => e.Idpartida)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("cos_partida");

                entity.HasIndex(e => new { e.Codigo, e.Activo })
                    .HasName("IX_cos_partida")
                    .IsUnique();

                entity.Property(e => e.Idpartida).HasColumnName("idpartida");

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasColumnName("activo")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasColumnName("codigo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConCuenta>(entity =>
            {
                entity.HasKey(e => e.Idcuenta)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("con_cuenta");

                entity.HasIndex(e => e.Clave)
                    .HasName("CtaClave");

                entity.HasIndex(e => e.Idcuenta)
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => new { e.Idapertura, e.Clave })
                    .HasName("IX_con_cuenta_clave")
                    .IsUnique();

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");

                entity.Property(e => e.Activa)
                    .IsRequired()
                    .HasColumnName("activa")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idapertura).HasColumnName("idapertura");
              
            });

            modelBuilder.Entity<CosCuentasAsociadas>(entity =>
            {
                entity.HasKey(e => new { e.Idcentro, e.Idetapa});

                entity.ToTable("cos_cuentasasociadas");

                entity.Property(e => e.Idcentro)
                    .HasColumnName("idcentro")
                    .ValueGeneratedNever();

                entity.Property(e => e.Idetapa)
                   .HasColumnName("idetapa")
                   .ValueGeneratedNever();
                              
            });

            modelBuilder.Entity<ConCuentamoneda>(entity =>
            {
                entity.HasKey(e => e.Idcuenta);

                entity.ToTable("con_cuentamoneda");

                entity.Property(e => e.Idcuenta)
                    .HasColumnName("idcuenta")
                    .ValueGeneratedNever();

                entity.Property(e => e.Idmoneda).HasColumnName("idmoneda");
               

            });

            modelBuilder.Entity<CosCentro>(entity =>
            {
                entity.HasKey(e => e.Idcentro)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("cos_centro");

                entity.HasIndex(e => e.Clave)
                    .HasName("CentrosClave");

                entity.HasIndex(e => e.Idcentro)
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => new { e.Clave, e.Idapertura })
                    .HasName("IX_cos_centro")
                    .IsUnique();

                entity.HasIndex(e => new { e.Idapertura, e.Clavenivel })
                    .HasName("IX_cos_centro_1")
                    .IsUnique();

                entity.HasIndex(e => new { e.Idapertura, e.Idcentro })
                    .HasName("IX_cos_centro_idapertura");

                entity.Property(e => e.Idcentro).HasColumnName("idcentro");

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasColumnName("activo")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Clavenivel)
                    .IsRequired()
                    .HasColumnName("clavenivel")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Idapertura).HasColumnName("idapertura");

                entity.Property(e => e.Saldocero)
                    .IsRequired()
                    .HasColumnName("saldocero")
                    .HasDefaultValueSql("(0)");
            });

            modelBuilder.Entity<CosElementogasto>(entity =>
            {
                entity.HasKey(e => e.Idelementogasto)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("cos_elementogasto");

                entity.HasIndex(e => new { e.Codigo, e.Activo })
                    .HasName("IX_cos_elementogasto")
                    .IsUnique();

                entity.Property(e => e.Idelementogasto).HasColumnName("idelementogasto");

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasColumnName("activo")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasColumnName("codigo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CosSubelementogasto>(entity =>
            {
                entity.HasKey(e => e.Idsubelemento)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("cos_subelementogasto");

                entity.HasIndex(e => new { e.Codigo, e.Activo })
                    .HasName("IX_cos_subelementogasto")
                    .IsUnique();

                entity.Property(e => e.Idsubelemento).HasColumnName("idsubelemento");

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasColumnName("activo")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasColumnName("codigo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Idelementogasto).HasColumnName("idelementogasto");

                entity.Property(e => e.Idpartida).HasColumnName("idpartida");

                entity.Property(e => e.Monnac)
                    .IsRequired()
                    .HasColumnName("monnac")
                    .HasDefaultValueSql("(1)");

                entity.HasOne(d => d.IdelementogastoNavigation)
                    .WithMany(p => p.CosSubelementogasto)
                    .HasForeignKey(d => d.Idelementogasto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_cos_subelementogasto_cos_elementogasto");
             
            });

            modelBuilder.Entity<GenMoneda>(entity =>
            {
                entity.HasKey(e => e.Idmoneda)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("gen_moneda");

                entity.HasIndex(e => e.Nombre)
                    .HasName("IX_gen_moneda_1")
                    .IsUnique();

                entity.HasIndex(e => e.Sigla)
                    .HasName("IX_gen_moneda")
                    .IsUnique();

                entity.Property(e => e.Idmoneda).HasColumnName("idmoneda");

                entity.Property(e => e.Factor)
                    .HasColumnName("factor")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.MantieneValorTasa)
                    .IsRequired()
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(30);

                entity.Property(e => e.Sigla)
                    .IsRequired()
                    .HasColumnName("sigla")
                    .HasMaxLength(3);

                entity.Property(e => e.Tipomoneda)
                    .HasColumnName("tipomoneda")
                    .HasDefaultValueSql("(3)");
                              
            
            });

            modelBuilder.Entity<GenPeriodo>(entity =>
            {
                entity.HasKey(e => e.Idperiodo)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("gen_periodo");

                entity.HasIndex(e => e.Idperiodo)
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => new { e.Idejercicio, e.Inicio })
                    .HasName("IX_gen_periodo");

                entity.Property(e => e.Idperiodo).HasColumnName("idperiodo");

                entity.Property(e => e.Enuso)
                    .IsRequired()
                    .HasColumnName("enuso")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Fin)
                    .HasColumnName("fin")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.Idejercicio).HasColumnName("idejercicio");

                entity.Property(e => e.Inicio)
                    .HasColumnName("inicio")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(30);

            
            });

            modelBuilder.Entity<GenUnidadcontable>(entity =>
            {
                entity.HasKey(e => e.Idunidad)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("gen_unidadcontable");

                entity.HasIndex(e => e.Codigo)
                    .HasName("IX_gen_unidadcontable")
                    .IsUnique();

                entity.HasIndex(e => e.Idunidad)
                    .HasName("IX_gen_unidadcontable_unidad")
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => e.Nombre)
                    .HasName("IX_gen_unidadcontable_1")
                    .IsUnique();

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasColumnName("activo")
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasColumnName("codigo")
                    .HasMaxLength(10);

                entity.Property(e => e.DirCorreo).HasMaxLength(150);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Iddpa).HasColumnName("iddpa");

                entity.Property(e => e.Idnae).HasColumnName("idnae");

                entity.Property(e => e.Idreup).HasColumnName("idreup");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(30);

            });

            modelBuilder.Entity<OptCuentaCentroSubPeriodo>(entity =>
            {
                entity.HasKey(e => new { e.Idunidad, e.Idperiodo, e.Idcuenta, e.Idcentro, e.Idsub });

                entity.ToTable("opt_cuenta_centro_sub_Periodo");

                entity.HasIndex(e => e.Clavecuenta);

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.Idperiodo).HasColumnName("idperiodo");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");

                entity.Property(e => e.Idcentro).HasColumnName("idcentro");

                entity.Property(e => e.Idsub).HasColumnName("idsub");

                entity.Property(e => e.Clavecentro)
                    .IsRequired()
                    .HasColumnName("clavecentro")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Clavecuenta)
                    .IsRequired()
                    .HasColumnName("clavecuenta")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Clavesub)
                    .IsRequired()
                    .HasColumnName("clavesub")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Credito)
                    .HasColumnName("credito")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Debito)
                    .HasColumnName("debito")
                    .HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.IdcentroNavigation)
                   .WithMany(p => p.OptCuentaCentroSubPeriodo)
                   .HasForeignKey(d => d.Idcentro)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_opt_cuenta_centro_sub_Periodo_cos_centro");

                entity.HasOne(d => d.IdcuentaNavigation)
                    .WithMany(p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey(d => d.Idcuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_sub_Periodo_con_cuenta");

                entity.HasOne(d => d.IdperiodoNavigation)
                    .WithMany(p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey(d => d.Idperiodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_sub_Periodo_gen_periodo");

                entity.HasOne(d => d.IdsubNavigation)
                    .WithMany(p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey(d => d.Idsub)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_sub_Periodo_cos_subelementogasto");

                entity.HasOne(d => d.IdunidadNavigation)
                    .WithMany(p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey(d => d.Idunidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_sub_Periodo_gen_unidadcontable");


            });

            modelBuilder.Entity<OptCuentaCentroPeriodo>(entity =>
            {
                entity.HasKey(e => new { e.Idunidad, e.Idperiodo, e.Idcuenta, e.Idcentro });

                entity.ToTable("opt_cuenta_centro_Periodo");

                entity.HasIndex(e => e.Clavecuenta);

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.Idperiodo).HasColumnName("idperiodo");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");

                entity.Property(e => e.Idcentro).HasColumnName("idcentro");

            

                entity.Property(e => e.Clavecentro)
                    .IsRequired()
                    .HasColumnName("clavecentro")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Clavecuenta)
                    .IsRequired()
                    .HasColumnName("clavecuenta")
                    .HasMaxLength(50)
                    .IsUnicode(false);

              

                entity.Property(e => e.Credito)
                    .HasColumnName("credito")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Debito)
                    .HasColumnName("debito")
                    .HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.IdcentroNavigation)
                   .WithMany(p => p.OptCuentaCentroPeriodo)
                   .HasForeignKey(d => d.Idcentro)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_opt_cuenta_centro_Periodo_cos_centro");

                entity.HasOne(d => d.IdcuentaNavigation)
                    .WithMany(p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey(d => d.Idcuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_Periodo_con_cuenta");

                entity.HasOne(d => d.IdperiodoNavigation)
                    .WithMany(p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey(d => d.Idperiodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_Periodo_gen_periodo");
               

                entity.HasOne(d => d.IdunidadNavigation)
                    .WithMany(p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey(d => d.Idunidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_centro_Periodo_gen_unidadcontable");


            });

            modelBuilder.Entity<OptCuentaPeriodo>(entity =>
            {
                entity.HasKey(e => new { e.Idunidad, e.Idperiodo, e.Idcuenta });

                entity.ToTable("opt_cuenta_Periodo");

                entity.HasIndex(e => e.Clave);

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.Idperiodo).HasColumnName("idperiodo");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");
               

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsUnicode(false);



                entity.Property(e => e.Credito)
                    .HasColumnName("credito")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Debito)
                    .HasColumnName("debito")
                    .HasColumnType("numeric(18, 2)");


                entity.HasOne(d => d.IdcuentaNavigation)
                    .WithMany(p => p.OptCuentaPeriodo)
                    .HasForeignKey(d => d.Idcuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_Periodo_con_cuenta");

                entity.HasOne(d => d.IdperiodoNavigation)
                    .WithMany(p => p.OptCuentaPeriodo)
                    .HasForeignKey(d => d.Idperiodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_Periodo_gen_periodo");


                entity.HasOne(d => d.IdunidadNavigation)
                    .WithMany(p => p.OptCuentaPeriodo)
                    .HasForeignKey(d => d.Idunidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_Periodo_gen_unidadcontable");


            });
            modelBuilder.Entity<NomAsiento>(entity =>
            {
                entity.HasKey(e => e.Idasiento);

                entity.ToTable("nom_asiento");

                entity.HasIndex(e => new { e.Idpase, e.Iddocumento })
                    .HasName("IX_nom_asiento_idpase");

                entity.Property(e => e.Idasiento).HasColumnName("idasiento");

                entity.Property(e => e.Asientogasto).HasColumnName("asientogasto");

                entity.Property(e => e.Asientonxp)
                    .IsRequired()
                    .HasColumnName("asientonxp")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Idcriterio).HasColumnName("idcriterio");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");

                entity.Property(e => e.Iddocumento).HasColumnName("iddocumento");

                entity.Property(e => e.Idmoneda).HasColumnName("idmoneda");

                entity.Property(e => e.Idpase).HasColumnName("idpase");

                entity.Property(e => e.Idtasa).HasColumnName("idtasa");

                entity.Property(e => e.IdtasaAnx).HasColumnName("idtasa_anx");

                entity.Property(e => e.NImporte)
                    .HasColumnName("n_importe")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NImporteAnx)
                    .HasColumnName("n_importe_anx")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NImportemo)
                    .HasColumnName("n_importemo")
                    .HasColumnType("numeric(18, 2)");



                entity.HasOne(d => d.IddocumentoNavigation)
                    .WithMany(p => p.NomAsiento)
                    .HasForeignKey(d => d.Iddocumento)
                    .HasConstraintName("FK_nom_asiento_nom_documento");


            });

            modelBuilder.Entity<NomAsientoGasto>(entity =>
            {
                entity.HasKey(e => new { e.Idasiento, e.Idcentro, e.Idsubelemento });

                entity.ToTable("nom_asiento_gasto");

                entity.Property(e => e.Idasiento).HasColumnName("idasiento");

                entity.Property(e => e.Idcentro).HasColumnName("idcentro");

                entity.Property(e => e.Idsubelemento).HasColumnName("idsubelemento");

                entity.Property(e => e.NImporte)
                    .HasColumnName("n_importe")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NImportemo)
                    .HasColumnName("n_importemo")
                    .HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.IdasientoNavigation)
                    .WithMany(p => p.NomAsientoGasto)
                    .HasForeignKey(d => d.Idasiento)
                    .HasConstraintName("FK_nom_asiento_gasto_nom_asiento");

            });
            modelBuilder.Entity<NomDocumento>(entity =>
            {
                entity.HasKey(e => e.Iddocumento)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("nom_documento");

                entity.HasIndex(e => e.Iddocumento)
                    .HasName("UQ__nom_documento__375BF910")
                    .IsUnique();

                entity.HasIndex(e => new { e.StrCodigo, e.Idperiodopago, e.Idtipodocumento })
                    .HasName("IX_nom_documento_codigo_periodo")
                    .IsUnique();

                entity.Property(e => e.Iddocumento).HasColumnName("iddocumento");



                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("datetime");

                entity.Property(e => e.Idactividad).HasColumnName("idactividad");

                entity.Property(e => e.Idestado).HasColumnName("idestado");

                entity.Property(e => e.Idmoneda).HasColumnName("idmoneda");

                entity.Property(e => e.Idperiodopago).HasColumnName("idperiodopago");

                entity.Property(e => e.Idtasa).HasColumnName("idtasa");

                entity.Property(e => e.Idtipodocumento).HasColumnName("idtipodocumento");

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.StrCodigo)
                    .HasColumnName("str_codigo")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.StrTitulo)
                    .HasColumnName("str_Titulo")
                    .HasMaxLength(100);




                entity.HasOne(d => d.IdperiodopagoNavigation)
                    .WithMany(p => p.NomDocumento)
                    .HasForeignKey(d => d.Idperiodopago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nom_documento_nom_periodopago");



                entity.HasOne(d => d.IdtipodocumentoNavigation)
                    .WithMany(p => p.NomDocumento)
                    .HasForeignKey(d => d.Idtipodocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nom_documento_nom_tipo_documento");


            });
            modelBuilder.Entity<NomPeriodopago>(entity =>
            {
                entity.HasKey(e => e.Idperiodopago);

                entity.ToTable("nom_periodopago");

                entity.HasIndex(e => e.Idperiodopago)
                    .HasName("UQ__nom_periodopago__41D98783")
                    .IsUnique();

                entity.Property(e => e.Idperiodopago).HasColumnName("idperiodopago");

                entity.Property(e => e.Coefsal)
                    .HasColumnName("coefsal")
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("(1)");



                entity.Property(e => e.Idprograma).HasColumnName("idprograma");

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.PeriodoFin)
                    .HasColumnName("periodo_fin")
                    .HasColumnType("datetime");

                entity.Property(e => e.PeriodoIni)
                    .HasColumnName("periodo_ini")
                    .HasColumnType("datetime");

                entity.Property(e => e.StrIdentificador)
                    .HasColumnName("str_identificador")
                    .HasMaxLength(30)
                    .IsUnicode(false);


            });
            modelBuilder.Entity<NomDocumentoDetallePago>(entity =>
            {
                entity.HasKey(e => e.Idlinea);

                entity.ToTable("nom_documento_detalle_pago");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cancelado)
                    .IsRequired()
                    .HasColumnName("cancelado")
                    .HasDefaultValueSql("(0)");


                entity.Property(e => e.Fechaalta)
                    .HasColumnName("fechaalta")
                    .HasColumnType("datetime");

                entity.Property(e => e.Idarea).HasColumnName("idarea");

                entity.Property(e => e.IdbasetiempoFondotiempo).HasColumnName("idbasetiempo_fondotiempo");

                entity.Property(e => e.Idcentrocosto).HasColumnName("idcentrocosto");

                entity.Property(e => e.Idcentrotrab).HasColumnName("idcentrotrab");

                entity.Property(e => e.Idconcepto).HasColumnName("idconcepto");

                entity.Property(e => e.Idcriterio).HasColumnName("idcriterio");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");

                entity.Property(e => e.Iddocumento).HasColumnName("iddocumento");

                entity.Property(e => e.Idlineaajustada).HasColumnName("idlineaajustada");

                entity.Property(e => e.Idpuestotrabajo).HasColumnName("idpuestotrabajo");

                entity.Property(e => e.Idtrabajador).HasColumnName("idtrabajador");

                entity.Property(e => e.NBasico)
                    .HasColumnName("n_basico")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NCobrar)
                    .HasColumnName("n_Cobrar")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NDias)
                    .HasColumnName("n_dias")
                    .HasColumnType("numeric(18, 3)");

                entity.Property(e => e.NDiasnoabsent)
                    .HasColumnName("n_diasnoabsent")
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.NFondodetiempo)
                    .HasColumnName("n_fondodetiempo")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NImporteAcumulado)
                    .HasColumnName("n_importeAcumulado")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NJornadalaboral)
                    .HasColumnName("n_jornadalaboral")
                    .HasColumnType("numeric(18, 6)");

                entity.Property(e => e.NLiquidado)
                    .HasColumnName("n_Liquidado")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTarifasalarial)
                    .HasColumnName("n_tarifasalarial")
                    .HasColumnType("numeric(18, 6)");

                entity.Property(e => e.NTiempoacumulado)
                    .HasColumnName("n_tiempoacumulado")
                    .HasColumnType("numeric(18, 6)");

                entity.Property(e => e.NTiempoentrado)
                    .HasColumnName("n_tiempoentrado")
                    .HasColumnType("numeric(18, 3)");

                entity.Property(e => e.NTotalBonificaciones)
                    .HasColumnName("n_TotalBonificaciones")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTotalCa)
                    .HasColumnName("n_TotalCA")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTotalDescuentos)
                    .HasColumnName("n_TotalDescuentos")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTotalImpSal)
                    .HasColumnName("n_TotalImpSal")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTotalPenalizaciones)
                    .HasColumnName("n_TotalPenalizaciones")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NTotalRetenciones)
                    .HasColumnName("n_TotalRetenciones")
                    .HasColumnType("numeric(18, 2)");


                entity.HasOne(d => d.IddocumentoNavigation)
                    .WithMany(p => p.NomDocumentoDetallePago)
                    .HasForeignKey(d => d.Iddocumento)
                    .HasConstraintName("FK_nom_documento_detalle_pago_nom_documento");

            });
            modelBuilder.Entity<NomTipoDocumento>(entity =>
            {
                entity.HasKey(e => e.Idtipodocumento);

                entity.ToTable("nom_tipo_documento");

                entity.Property(e => e.Idtipodocumento)
                    .HasColumnName("idtipodocumento")
                    .ValueGeneratedNever();

                entity.Property(e => e.StrDescripcion)
                    .HasColumnName("str_descripcion")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OptCuentaPeriodoOK>(entity =>
            {
                entity.HasKey(e => new { e.Idpasecuenta, e.Idunidad, e.Idperiodo, e.Idcuenta });

                entity.ToTable("opt_cuenta_PeriodoOK");

                entity.HasIndex(e => e.Clavecuenta);

                entity.Property(e => e.Idpasecuenta).HasColumnName("Idpasecuenta");

                entity.Property(e => e.Idunidad).HasColumnName("idunidad");

                entity.Property(e => e.Idperiodo).HasColumnName("idperiodo");

                entity.Property(e => e.Idcuenta).HasColumnName("idcuenta");


                entity.Property(e => e.Clavecuenta)
                    .IsRequired()
                    .HasColumnName("clavecuenta")
                    .HasMaxLength(50)
                    .IsUnicode(false);



                entity.Property(e => e.Importe)
                    .HasColumnName("importe")
                    .HasColumnType("numeric(18, 2)");

              

                entity.HasOne(d => d.IdcuentaNavigation)
                    .WithMany(p => p.OptCuentaPeriodoOK)
                    .HasForeignKey(d => d.Idcuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_PeriodoOK_con_cuenta");

                entity.HasOne(d => d.IdperiodoNavigation)
                    .WithMany(p => p.OptCuentaPeriodoOK)
                    .HasForeignKey(d => d.Idperiodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_PeriodoOK_gen_periodo");


                entity.HasOne(d => d.IdunidadNavigation)
                    .WithMany(p => p.OptCuentaPeriodoOK)
                    .HasForeignKey(d => d.Idunidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_opt_cuenta_PeriodoOK_gen_unidadcontable");


            });

            modelBuilder.Entity<ConCuentanat>(entity =>
            {
                entity.HasKey(e => e.Idcuenta)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("con_cuentanat");

                entity.HasIndex(e => e.Idcuenta)
                    .HasName("IX_con_cuentanat")
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Idcuenta)
                    .HasColumnName("idcuenta")
                    .ValueGeneratedNever();

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Naturaleza).HasColumnName("naturaleza");

            });
        }



    }
}
