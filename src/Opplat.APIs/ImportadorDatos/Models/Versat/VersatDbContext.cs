using Microsoft.EntityFrameworkCore;

namespace ImportadorDatos.Models.Versat {
    public class VersatDbContext : DbContext {
        public VersatDbContext (DbContextOptions<VersatDbContext> options) : base (options) { }
        public DbSet<ImportadorDatos.Models.Versat.GenPeriodo> GenPeriodo { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.ConCuentanat> ConCuentanat { get; set; }
        public virtual DbSet<ImportadorDatos.Models.Versat.ConApertura> ConApertura { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.ConCuenta> ConCuenta { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.GenUnidadcontable> GenUnidadcontable { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.ConCuentamoneda> ConCuentamoneda { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.GenFormato> GenFormato { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.GenMascara> GenMascara { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.GenMoneda> GenMoneda { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.CosCuentasAsociadas> CosCuentasAsociadas { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.CosPartida> CosPartida { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.OptCuentaCentroPeriodo> OptCuentaCentroPeriodo { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.OptCuentaPeriodoOK> OptCuentaPeriodoOK { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomAsiento> NomAsiento { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomAsientoGasto> NomAsientoGasto { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomDocumento> NomDocumento { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomDocumentoDetallePago> NomDocumentoDetallePago { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomPeriodopago> NomPeriodopago { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.NomTipoDocumento> NomTipoDocumento { get; set; }
        public DbQuery<ImportadorDatos.Models.Versat.Con_Cuentadescrip> con_cuentadescrip { get; set; }
        public DbQuery<ImportadorDatos.Models.Versat.ConCuentanatur> con_cuentanatur { get; set; }
        public DbQuery<ImportadorDatos.Models.Versat.AsientosView> nom_vw_asientos { get; set; }

        public virtual DbSet<GenUsuario> GenUsuario { get; set; }
        public virtual DbSet<ConPase> ConPase { get; set; }
        public virtual DbSet<ConComprobante> ConComprobante { get; set; }
        public virtual DbSet<ConComprobanteoperacion> ConComprobanteoperacion { get; set; }
        public virtual DbSet<GenTrabajador> GenTrabajador { get; set; }
        public virtual DbSet<GenEntidad> GenEntidad { get; set; }
        public virtual DbSet<GenCtaBancoEntidad> GenCtaBancoEntidad { get; set; }
        public virtual DbSet<GenSucursalBanco> GenSucursalBanco { get; set; }
        public virtual DbSet<GenClasifBanco> GenClasifBanco { get; set; }

        //gastos
        public virtual DbSet<ConRegistroanexo> ConRegistroanexo { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.CosElementogasto> CosElementogasto { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.CosSubelementogasto> CosSubelementogasto { get; set; }
        public virtual DbSet<CosRegistrogasto> CosRegistrogasto { get; set; }
        public virtual DbSet<CosPasecentro> CosPasecentro { get; set; }
        public virtual DbSet<CosPasesubelemento> CosPasesubelemento { get; set; }
        public DbSet<ImportadorDatos.Models.Versat.CosCentro> CosCentro { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {

        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.HasAnnotation ("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Query<Con_Cuentadescrip> ();
            modelBuilder.Query<ConCuentanatur> ();

            modelBuilder.Entity<CosPartida> (entity => {
                entity.HasKey (e => e.Idpartida)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_partida");

                entity.HasIndex (e => new { e.Codigo, e.Activo })
                    .HasName ("IX_cos_partida")
                    .IsUnique ();

                entity.Property (e => e.Idpartida).HasColumnName ("idpartida");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);
            });

            modelBuilder.Entity<ConCuenta> (entity => {
                entity.HasKey (e => e.Idcuenta)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_cuenta");

                entity.HasIndex (e => e.Clave)
                    .HasName ("CtaClave");

                entity.HasIndex (e => e.Idcuenta)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => new { e.Idapertura, e.Clave })
                    .HasName ("IX_con_cuenta_clave")
                    .IsUnique ();

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Activa)
                    .IsRequired ()
                    .HasColumnName ("activa")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Clave)
                    .IsRequired ()
                    .HasColumnName ("clave")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Idapertura).HasColumnName ("idapertura");

            });

            modelBuilder.Entity<CosCuentasAsociadas> (entity => {
                entity.HasKey (e => new { e.Idcentro, e.Idetapa });

                entity.ToTable ("cos_cuentasasociadas");

                entity.Property (e => e.Idcentro)
                    .HasColumnName ("idcentro")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Idetapa)
                    .HasColumnName ("idetapa")
                    .ValueGeneratedNever ();

            });

            modelBuilder.Entity<ConCuentamoneda> (entity => {
                entity.HasKey (e => e.Idcuenta);

                entity.ToTable ("con_cuentamoneda");

                entity.Property (e => e.Idcuenta)
                    .HasColumnName ("idcuenta")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Idmoneda).HasColumnName ("idmoneda");

            });

            modelBuilder.Entity<CosCentro> (entity => {
                entity.HasKey (e => e.Idcentro)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_centro");

                entity.HasIndex (e => e.Clave)
                    .HasName ("CentrosClave");

                entity.HasIndex (e => e.Idcentro)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => new { e.Clave, e.Idapertura })
                    .HasName ("IX_cos_centro")
                    .IsUnique ();

                entity.HasIndex (e => new { e.Idapertura, e.Clavenivel })
                    .HasName ("IX_cos_centro_1")
                    .IsUnique ();

                entity.HasIndex (e => new { e.Idapertura, e.Idcentro })
                    .HasName ("IX_cos_centro_idapertura");

                entity.Property (e => e.Idcentro).HasColumnName ("idcentro");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Clave)
                    .IsRequired ()
                    .HasColumnName ("clave")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Clavenivel)
                    .IsRequired ()
                    .HasColumnName ("clavenivel")
                    .HasMaxLength (25)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);

                entity.Property (e => e.Idapertura).HasColumnName ("idapertura");

                entity.Property (e => e.Saldocero)
                    .IsRequired ()
                    .HasColumnName ("saldocero")
                    .HasDefaultValueSql ("(0)");
            });

            modelBuilder.Entity<CosElementogasto> (entity => {
                entity.HasKey (e => e.Idelementogasto)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_elementogasto");

                entity.HasIndex (e => new { e.Codigo, e.Activo })
                    .HasName ("IX_cos_elementogasto")
                    .IsUnique ();

                entity.Property (e => e.Idelementogasto).HasColumnName ("idelementogasto");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);
            });

            modelBuilder.Entity<CosSubelementogasto> (entity => {
                entity.HasKey (e => e.Idsubelemento)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_subelementogasto");

                entity.HasIndex (e => new { e.Codigo, e.Activo })
                    .HasName ("IX_cos_subelementogasto")
                    .IsUnique ();

                entity.Property (e => e.Idsubelemento).HasColumnName ("idsubelemento");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);

                entity.Property (e => e.Idelementogasto).HasColumnName ("idelementogasto");

                entity.Property (e => e.Idpartida).HasColumnName ("idpartida");

                entity.Property (e => e.Monnac)
                    .IsRequired ()
                    .HasColumnName ("monnac")
                    .HasDefaultValueSql ("(1)");

                entity.HasOne (d => d.IdelementogastoNavigation)
                    .WithMany (p => p.CosSubelementogasto)
                    .HasForeignKey (d => d.Idelementogasto)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_subelementogasto_cos_elementogasto");

            });

            modelBuilder.Entity<GenMoneda> (entity => {
                entity.HasKey (e => e.Idmoneda)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_moneda");

                entity.HasIndex (e => e.Nombre)
                    .HasName ("IX_gen_moneda_1")
                    .IsUnique ();

                entity.HasIndex (e => e.Sigla)
                    .HasName ("IX_gen_moneda")
                    .IsUnique ();

                entity.Property (e => e.Idmoneda).HasColumnName ("idmoneda");

                entity.Property (e => e.Factor)
                    .HasColumnName ("factor")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.MantieneValorTasa)
                    .IsRequired ()
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.Nombre)
                    .IsRequired ()
                    .HasColumnName ("nombre")
                    .HasMaxLength (30);

                entity.Property (e => e.Sigla)
                    .IsRequired ()
                    .HasColumnName ("sigla")
                    .HasMaxLength (3);

                entity.Property (e => e.Tipomoneda)
                    .HasColumnName ("tipomoneda")
                    .HasDefaultValueSql ("(3)");

            });

            modelBuilder.Entity<GenPeriodo> (entity => {
                entity.HasKey (e => e.Idperiodo)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_periodo");

                entity.HasIndex (e => e.Idperiodo)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => new { e.Idejercicio, e.Inicio })
                    .HasName ("IX_gen_periodo");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Enuso)
                    .IsRequired ()
                    .HasColumnName ("enuso")
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.Fin)
                    .HasColumnName ("fin")
                    .HasColumnType ("smalldatetime");

                entity.Property (e => e.Idejercicio).HasColumnName ("idejercicio");

                entity.Property (e => e.Inicio)
                    .HasColumnName ("inicio")
                    .HasColumnType ("smalldatetime");

                entity.Property (e => e.Nombre)
                    .IsRequired ()
                    .HasColumnName ("nombre")
                    .HasMaxLength (30);

            });

            modelBuilder.Entity<GenUnidadcontable> (entity => {
                entity.HasKey (e => e.Idunidad)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_unidadcontable");

                entity.HasIndex (e => e.Codigo)
                    .HasName ("IX_gen_unidadcontable")
                    .IsUnique ();

                entity.HasIndex (e => e.Idunidad)
                    .HasName ("IX_gen_unidadcontable_unidad")
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => e.Nombre)
                    .HasName ("IX_gen_unidadcontable_1")
                    .IsUnique ();

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (10);

                entity.Property (e => e.DirCorreo).HasMaxLength (150);

                entity.Property (e => e.Direccion)
                    .HasMaxLength (150)
                    .IsUnicode (false);

                entity.Property (e => e.Iddpa).HasColumnName ("iddpa");

                entity.Property (e => e.Idnae).HasColumnName ("idnae");

                entity.Property (e => e.Idreup).HasColumnName ("idreup");

                entity.Property (e => e.Nombre)
                    .IsRequired ()
                    .HasColumnName ("nombre")
                    .HasMaxLength (30);

            });

            modelBuilder.Entity<OptCuentaCentroSubPeriodo> (entity => {
                entity.HasKey (e => new { e.Idunidad, e.Idperiodo, e.Idcuenta, e.Idcentro, e.Idsub });

                entity.ToTable ("opt_cuenta_centro_sub_Periodo");

                entity.HasIndex (e => e.Clavecuenta);

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Idcentro).HasColumnName ("idcentro");

                entity.Property (e => e.Idsub).HasColumnName ("idsub");

                entity.Property (e => e.Clavecentro)
                    .IsRequired ()
                    .HasColumnName ("clavecentro")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Clavecuenta)
                    .IsRequired ()
                    .HasColumnName ("clavecuenta")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Clavesub)
                    .IsRequired ()
                    .HasColumnName ("clavesub")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Credito)
                    .HasColumnName ("credito")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Debito)
                    .HasColumnName ("debito")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcentroNavigation)
                    .WithMany (p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey (d => d.Idcentro)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_sub_Periodo_cos_centro");

                entity.HasOne (d => d.IdcuentaNavigation)
                    .WithMany (p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey (d => d.Idcuenta)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_sub_Periodo_con_cuenta");

                entity.HasOne (d => d.IdperiodoNavigation)
                    .WithMany (p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey (d => d.Idperiodo)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_sub_Periodo_gen_periodo");

                entity.HasOne (d => d.IdsubNavigation)
                    .WithMany (p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey (d => d.Idsub)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_sub_Periodo_cos_subelementogasto");

                entity.HasOne (d => d.IdunidadNavigation)
                    .WithMany (p => p.OptCuentaCentroSubPeriodo)
                    .HasForeignKey (d => d.Idunidad)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_sub_Periodo_gen_unidadcontable");

            });

            modelBuilder.Entity<OptCuentaCentroPeriodo> (entity => {
                entity.HasKey (e => new { e.Idunidad, e.Idperiodo, e.Idcuenta, e.Idcentro });

                entity.ToTable ("opt_cuenta_centro_Periodo");

                entity.HasIndex (e => e.Clavecuenta);

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Idcentro).HasColumnName ("idcentro");

                entity.Property (e => e.Clavecentro)
                    .IsRequired ()
                    .HasColumnName ("clavecentro")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Clavecuenta)
                    .IsRequired ()
                    .HasColumnName ("clavecuenta")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Credito)
                    .HasColumnName ("credito")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Debito)
                    .HasColumnName ("debito")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcentroNavigation)
                    .WithMany (p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey (d => d.Idcentro)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_Periodo_cos_centro");

                entity.HasOne (d => d.IdcuentaNavigation)
                    .WithMany (p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey (d => d.Idcuenta)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_Periodo_con_cuenta");

                entity.HasOne (d => d.IdperiodoNavigation)
                    .WithMany (p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey (d => d.Idperiodo)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_Periodo_gen_periodo");

                entity.HasOne (d => d.IdunidadNavigation)
                    .WithMany (p => p.OptCuentaCentroPeriodo)
                    .HasForeignKey (d => d.Idunidad)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_centro_Periodo_gen_unidadcontable");

            });

            modelBuilder.Entity<OptCuentaPeriodo> (entity => {
                entity.HasKey (e => new { e.Idunidad, e.Idperiodo, e.Idcuenta });

                entity.ToTable ("opt_cuenta_Periodo");

                entity.HasIndex (e => e.Clave);

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Clave)
                    .IsRequired ()
                    .HasColumnName ("clave")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Credito)
                    .HasColumnName ("credito")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Debito)
                    .HasColumnName ("debito")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcuentaNavigation)
                    .WithMany (p => p.OptCuentaPeriodo)
                    .HasForeignKey (d => d.Idcuenta)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_Periodo_con_cuenta");

                entity.HasOne (d => d.IdperiodoNavigation)
                    .WithMany (p => p.OptCuentaPeriodo)
                    .HasForeignKey (d => d.Idperiodo)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_Periodo_gen_periodo");

                entity.HasOne (d => d.IdunidadNavigation)
                    .WithMany (p => p.OptCuentaPeriodo)
                    .HasForeignKey (d => d.Idunidad)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_Periodo_gen_unidadcontable");

            });
            modelBuilder.Entity<NomAsiento> (entity => {
                entity.HasKey (e => e.Idasiento);

                entity.ToTable ("nom_asiento");

                entity.HasIndex (e => new { e.Idpase, e.Iddocumento })
                    .HasName ("IX_nom_asiento_idpase");

                entity.Property (e => e.Idasiento).HasColumnName ("idasiento");

                entity.Property (e => e.Asientogasto).HasColumnName ("asientogasto");

                entity.Property (e => e.Asientonxp)
                    .IsRequired ()
                    .HasColumnName ("asientonxp")
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.Idcriterio).HasColumnName ("idcriterio");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Iddocumento).HasColumnName ("iddocumento");

                entity.Property (e => e.Idmoneda).HasColumnName ("idmoneda");

                entity.Property (e => e.Idpase).HasColumnName ("idpase");

                entity.Property (e => e.Idtasa).HasColumnName ("idtasa");

                entity.Property (e => e.IdtasaAnx).HasColumnName ("idtasa_anx");

                entity.Property (e => e.NImporte)
                    .HasColumnName ("n_importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NImporteAnx)
                    .HasColumnName ("n_importe_anx")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NImportemo)
                    .HasColumnName ("n_importemo")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IddocumentoNavigation)
                    .WithMany (p => p.NomAsiento)
                    .HasForeignKey (d => d.Iddocumento)
                    .HasConstraintName ("FK_nom_asiento_nom_documento");

            });

            modelBuilder.Entity<NomAsientoGasto> (entity => {
                entity.HasKey (e => new { e.Idasiento, e.Idcentro, e.Idsubelemento });

                entity.ToTable ("nom_asiento_gasto");

                entity.Property (e => e.Idasiento).HasColumnName ("idasiento");

                entity.Property (e => e.Idcentro).HasColumnName ("idcentro");

                entity.Property (e => e.Idsubelemento).HasColumnName ("idsubelemento");

                entity.Property (e => e.NImporte)
                    .HasColumnName ("n_importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NImportemo)
                    .HasColumnName ("n_importemo")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdasientoNavigation)
                    .WithMany (p => p.NomAsientoGasto)
                    .HasForeignKey (d => d.Idasiento)
                    .HasConstraintName ("FK_nom_asiento_gasto_nom_asiento");

            });
            modelBuilder.Entity<NomDocumento> (entity => {
                entity.HasKey (e => e.Iddocumento)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("nom_documento");

                entity.HasIndex (e => e.Iddocumento)
                    .HasName ("UQ__nom_documento__375BF910")
                    .IsUnique ();

                entity.HasIndex (e => new { e.StrCodigo, e.Idperiodopago, e.Idtipodocumento })
                    .HasName ("IX_nom_documento_codigo_periodo")
                    .IsUnique ();

                entity.Property (e => e.Iddocumento).HasColumnName ("iddocumento");

                entity.Property (e => e.Fecha)
                    .HasColumnName ("fecha")
                    .HasColumnType ("datetime");

                entity.Property (e => e.Idactividad).HasColumnName ("idactividad");

                entity.Property (e => e.Idestado).HasColumnName ("idestado");

                entity.Property (e => e.Idmoneda).HasColumnName ("idmoneda");

                entity.Property (e => e.Idperiodopago).HasColumnName ("idperiodopago");

                entity.Property (e => e.Idtasa).HasColumnName ("idtasa");

                entity.Property (e => e.Idtipodocumento).HasColumnName ("idtipodocumento");

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.StrCodigo)
                    .HasColumnName ("str_codigo")
                    .HasMaxLength (5)
                    .IsUnicode (false);

                entity.Property (e => e.StrTitulo)
                    .HasColumnName ("str_Titulo")
                    .HasMaxLength (100);

                entity.HasOne (d => d.IdperiodopagoNavigation)
                    .WithMany (p => p.NomDocumento)
                    .HasForeignKey (d => d.Idperiodopago)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_nom_documento_nom_periodopago");

                entity.HasOne (d => d.IdtipodocumentoNavigation)
                    .WithMany (p => p.NomDocumento)
                    .HasForeignKey (d => d.Idtipodocumento)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_nom_documento_nom_tipo_documento");

            });
            modelBuilder.Entity<NomPeriodopago> (entity => {
                entity.HasKey (e => e.Idperiodopago);

                entity.ToTable ("nom_periodopago");

                entity.HasIndex (e => e.Idperiodopago)
                    .HasName ("UQ__nom_periodopago__41D98783")
                    .IsUnique ();

                entity.Property (e => e.Idperiodopago).HasColumnName ("idperiodopago");

                entity.Property (e => e.Coefsal)
                    .HasColumnName ("coefsal")
                    .HasColumnType ("numeric(18, 6)")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Idprograma).HasColumnName ("idprograma");

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.PeriodoFin)
                    .HasColumnName ("periodo_fin")
                    .HasColumnType ("datetime");

                entity.Property (e => e.PeriodoIni)
                    .HasColumnName ("periodo_ini")
                    .HasColumnType ("datetime");

                entity.Property (e => e.StrIdentificador)
                    .HasColumnName ("str_identificador")
                    .HasMaxLength (30)
                    .IsUnicode (false);

            });
            modelBuilder.Entity<NomDocumentoDetallePago> (entity => {
                entity.HasKey (e => e.Idlinea);

                entity.ToTable ("nom_documento_detalle_pago");

                entity.Property (e => e.Idlinea)
                    .HasColumnName ("idlinea")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Cancelado)
                    .IsRequired ()
                    .HasColumnName ("cancelado")
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.Fechaalta)
                    .HasColumnName ("fechaalta")
                    .HasColumnType ("datetime");

                entity.Property (e => e.Idarea).HasColumnName ("idarea");

                entity.Property (e => e.IdbasetiempoFondotiempo).HasColumnName ("idbasetiempo_fondotiempo");

                entity.Property (e => e.Idcentrocosto).HasColumnName ("idcentrocosto");

                entity.Property (e => e.Idcentrotrab).HasColumnName ("idcentrotrab");

                entity.Property (e => e.Idconcepto).HasColumnName ("idconcepto");

                entity.Property (e => e.Idcriterio).HasColumnName ("idcriterio");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Iddocumento).HasColumnName ("iddocumento");

                entity.Property (e => e.Idlineaajustada).HasColumnName ("idlineaajustada");

                entity.Property (e => e.Idpuestotrabajo).HasColumnName ("idpuestotrabajo");

                entity.Property (e => e.Idtrabajador).HasColumnName ("idtrabajador");

                entity.Property (e => e.NBasico)
                    .HasColumnName ("n_basico")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NCobrar)
                    .HasColumnName ("n_Cobrar")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NDias)
                    .HasColumnName ("n_dias")
                    .HasColumnType ("numeric(18, 3)");

                entity.Property (e => e.NDiasnoabsent)
                    .HasColumnName ("n_diasnoabsent")
                    .HasColumnType ("numeric(18, 2)")
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.NFondodetiempo)
                    .HasColumnName ("n_fondodetiempo")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NImporteAcumulado)
                    .HasColumnName ("n_importeAcumulado")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NJornadalaboral)
                    .HasColumnName ("n_jornadalaboral")
                    .HasColumnType ("numeric(18, 6)");

                entity.Property (e => e.NLiquidado)
                    .HasColumnName ("n_Liquidado")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTarifasalarial)
                    .HasColumnName ("n_tarifasalarial")
                    .HasColumnType ("numeric(18, 6)");

                entity.Property (e => e.NTiempoacumulado)
                    .HasColumnName ("n_tiempoacumulado")
                    .HasColumnType ("numeric(18, 6)");

                entity.Property (e => e.NTiempoentrado)
                    .HasColumnName ("n_tiempoentrado")
                    .HasColumnType ("numeric(18, 3)");

                entity.Property (e => e.NTotalBonificaciones)
                    .HasColumnName ("n_TotalBonificaciones")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTotalCa)
                    .HasColumnName ("n_TotalCA")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTotalDescuentos)
                    .HasColumnName ("n_TotalDescuentos")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTotalImpSal)
                    .HasColumnName ("n_TotalImpSal")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTotalPenalizaciones)
                    .HasColumnName ("n_TotalPenalizaciones")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.NTotalRetenciones)
                    .HasColumnName ("n_TotalRetenciones")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IddocumentoNavigation)
                    .WithMany (p => p.NomDocumentoDetallePago)
                    .HasForeignKey (d => d.Iddocumento)
                    .HasConstraintName ("FK_nom_documento_detalle_pago_nom_documento");

            });
            modelBuilder.Entity<NomTipoDocumento> (entity => {
                entity.HasKey (e => e.Idtipodocumento);

                entity.ToTable ("nom_tipo_documento");

                entity.Property (e => e.Idtipodocumento)
                    .HasColumnName ("idtipodocumento")
                    .ValueGeneratedNever ();

                entity.Property (e => e.StrDescripcion)
                    .HasColumnName ("str_descripcion")
                    .HasMaxLength (50)
                    .IsUnicode (false);
            });

            modelBuilder.Entity<OptCuentaPeriodoOK> (entity => {
                entity.HasKey (e => new { e.Idpasecuenta, e.Idunidad, e.Idperiodo, e.Idcuenta });

                entity.ToTable ("opt_cuenta_PeriodoOK");

                entity.HasIndex (e => e.Clavecuenta);

                entity.Property (e => e.Idpasecuenta).HasColumnName ("Idpasecuenta");

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Clavecuenta)
                    .IsRequired ()
                    .HasColumnName ("clavecuenta")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Importe)
                    .HasColumnName ("importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcuentaNavigation)
                    .WithMany (p => p.OptCuentaPeriodoOK)
                    .HasForeignKey (d => d.Idcuenta)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_PeriodoOK_con_cuenta");

                entity.HasOne (d => d.IdperiodoNavigation)
                    .WithMany (p => p.OptCuentaPeriodoOK)
                    .HasForeignKey (d => d.Idperiodo)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_PeriodoOK_gen_periodo");

                entity.HasOne (d => d.IdunidadNavigation)
                    .WithMany (p => p.OptCuentaPeriodoOK)
                    .HasForeignKey (d => d.Idunidad)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_opt_cuenta_PeriodoOK_gen_unidadcontable");

            });

            modelBuilder.Entity<ConCuentanat> (entity => {
                entity.HasKey (e => e.Idcuenta)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_cuentanat");

                entity.HasIndex (e => e.Idcuenta)
                    .HasName ("IX_con_cuentanat")
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.Idcuenta)
                    .HasColumnName ("idcuenta")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Clave)
                    .IsRequired ()
                    .HasColumnName ("clave")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);

                entity.Property (e => e.Naturaleza).HasColumnName ("naturaleza");

            });

            modelBuilder.Entity<ConApertura> (entity => {
                entity.HasKey (e => e.Idapertura)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_apertura");

                entity.HasIndex (e => e.Idapertura)
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.Idapertura).HasColumnName ("idapertura");

                entity.Property (e => e.Idmascara).HasColumnName ("idmascara");

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Tipo).HasColumnName ("tipo");

                entity.HasOne (d => d.IdmascaraNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idmascara)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_apertura_gen_mascara");

                entity.HasOne (d => d.IdunidadNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idunidad)
                    .HasConstraintName ("FK_con_apertura_gen_unidadcontable");
            });

            modelBuilder.Entity<GenFormato> (entity => {
                entity.HasKey (e => e.Idformato)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_formato");

                entity.HasIndex (e => e.Nombre)
                    .HasName ("IX_gen_formato")
                    .IsUnique ();

                entity.Property (e => e.Idformato).HasColumnName ("idformato");

                entity.Property (e => e.Enuso)
                    .IsRequired ()
                    .HasColumnName ("enuso")
                    .HasDefaultValueSql ("(0)");

                entity.Property (e => e.Longitud).HasColumnName ("longitud");

                entity.Property (e => e.Nombre)
                    .IsRequired ()
                    .HasColumnName ("nombre")
                    .HasMaxLength (30);

                entity.Property (e => e.Separador)
                    .IsRequired ()
                    .HasColumnName ("separador")
                    .HasMaxLength (1)
                    .IsUnicode (false);
            });

            modelBuilder.Entity<GenMascara> (entity => {
                entity.HasKey (e => e.Idmascara)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_mascara");

                entity.HasIndex (e => new { e.Idformato, e.Posicion })
                    .HasName ("IX_gen_mascara");

                entity.Property (e => e.Idmascara).HasColumnName ("idmascara");

                entity.Property (e => e.Abreviatura)
                    .HasColumnName ("abreviatura")
                    .HasMaxLength (5);

                entity.Property (e => e.Idformato).HasColumnName ("idformato");

                entity.Property (e => e.Longitud).HasColumnName ("longitud");

                entity.Property (e => e.Nombre)
                    .IsRequired ()
                    .HasColumnName ("nombre")
                    .HasMaxLength (30);

                entity.Property (e => e.Posicion).HasColumnName ("posicion");

                entity.HasOne (d => d.IdformatoNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idformato)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_gen_mascara_gen_formato");
            });

            modelBuilder.Entity<GenUsuario> (entity => {
                entity.HasKey (e => e.Idusuario)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_usuario");

                entity.HasIndex (e => e.Idusuario)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => e.Loginusuario)
                    .HasName ("IX_gen_usuario")
                    .IsUnique ();

                entity.Property (e => e.Idusuario).HasColumnName ("idusuario");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Loginusuario)
                    .IsRequired ()
                    .HasColumnName ("loginusuario")
                    .HasMaxLength (128);

                entity.Property (e => e.Nombre)
                    .HasColumnName ("nombre")
                    .HasMaxLength (128);

                entity.Property (e => e.Tipo).HasColumnName ("tipo");
            });

            modelBuilder.Entity<ConComprobante> (entity => {
                entity.HasKey (e => e.Idcomprobante)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_comprobante");

                entity.HasIndex (e => e.Idcomprobante)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => e.Idunidad)
                    .HasName ("IX_con_comprobante_unidad");

                entity.HasIndex (e => new { e.Idunidad, e.Idcomprobante })
                    .HasName ("IX_con_comprobante");

                entity.Property (e => e.Idcomprobante).HasColumnName ("idcomprobante");

                entity.Property (e => e.Credito)
                    .HasColumnName ("credito")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Debito)
                    .HasColumnName ("debito")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (1000)
                    .IsUnicode (false);

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.Property (e => e.Sumaclave)
                    .HasColumnName ("sumaclave")
                    .HasMaxLength (50)
                    .IsUnicode (false);
            });

            modelBuilder.Entity<ConComprobanteoperacion> (entity => {
                entity.HasKey (e => e.Idcomprobante)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_comprobanteoperacion");

                entity.HasIndex (e => e.Fecha);

                entity.HasIndex (e => e.Idcomprobante)
                    .ForSqlServerIsClustered ();

                entity.HasIndex (e => e.Idestado);

                entity.HasIndex (e => e.Idsubsistema);

                entity.HasIndex (e => e.Idusuario);

                entity.HasIndex (e => e.Numero);

                entity.HasIndex (e => new { e.Fecha, e.Idperiodo })
                    .HasName ("IX_con_comprobanteoperacion_optimizacion");

                entity.Property (e => e.Idcomprobante)
                    .HasColumnName ("idcomprobante")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Comentario)
                    .HasColumnName ("comentario")
                    .HasMaxLength (7000)
                    .IsUnicode (false)
                    .HasDefaultValueSql ("(' ')");

                entity.Property (e => e.Fecha)
                    .HasColumnName ("fecha")
                    .HasColumnType ("smalldatetime");

                entity.Property (e => e.Idestado).HasColumnName ("idestado");

                entity.Property (e => e.Idperiodo).HasColumnName ("idperiodo");

                entity.Property (e => e.Idsubsistema).HasColumnName ("idsubsistema");

                entity.Property (e => e.Idusuario).HasColumnName ("idusuario");

                entity.Property (e => e.Numero).HasColumnName ("numero");

                entity.HasOne (d => d.IdcomprobanteNavigation)
                    .WithOne (p => p.ConComprobanteoperacion)
                    .HasForeignKey<ConComprobanteoperacion> (d => d.Idcomprobante)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_comprobanteoperacion_con_comprobante");

                entity.HasOne (d => d.IdperiodoNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idperiodo)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_comprobanteoperacion_gen_periodo");

                entity.HasOne (d => d.IdusuarioNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idusuario)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_comprobanteoperacion_gen_usuario");
            });

            modelBuilder.Entity<ConPase> (entity => {
                entity.HasKey (e => e.Idpase)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_pase");

                entity.HasIndex (e => e.Idcomprobante);

                entity.HasIndex (e => e.Idcuenta);

                entity.HasIndex (e => new { e.Idcomprobante, e.Idcuenta })
                    .HasName ("IX_con_pase_asiento");

                entity.HasIndex (e => new { e.Idpase, e.Idcomprobante })
                    .HasName ("IX_con_pase_principal")
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.Idpase).HasColumnName ("idpase");

                entity.Property (e => e.Idcomprobante).HasColumnName ("idcomprobante");

                entity.Property (e => e.Idcuenta).HasColumnName ("idcuenta");

                entity.Property (e => e.Importe)
                    .HasColumnName ("importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcomprobanteNavigation)
                    .WithMany (p => p.ConPase)
                    .HasForeignKey (d => d.Idcomprobante)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_pase_con_comprobante");

                entity.HasOne (d => d.IdcuentaNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idcuenta)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_pase_con_cuenta");
            });

            modelBuilder.Entity<GenTrabajador> (entity => {
                entity.HasKey (e => e.Idtrabajador)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_trabajador");

                entity.HasIndex (e => e.Codigo)
                    .HasName ("IX_gen_trabajador")
                    .IsUnique ();

                entity.HasIndex (e => e.Idtrabajador)
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.Idtrabajador).HasColumnName ("idtrabajador");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Apellido1).HasMaxLength (50);

                entity.Property (e => e.Apellido2).HasMaxLength (50);

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (6);

                entity.Property (e => e.Direccion)
                    .HasColumnName ("direccion")
                    .HasMaxLength (150);

                entity.Property (e => e.Nombre)
                    .HasColumnName ("nombre")
                    .HasMaxLength (252)
                    .HasComputedColumnSql ("(rtrim(isnull([nombres],'')) + ' ' + rtrim(isnull([Apellido1],'')) + ' ' + rtrim(isnull([Apellido2],'')))");

                entity.Property (e => e.Nombres)
                    .HasColumnName ("nombres")
                    .HasMaxLength (150);

                entity.Property (e => e.Numident)
                    .HasColumnName ("numident")
                    .HasMaxLength (11);
            });

            modelBuilder.Entity<GenArea> (entity => {
                entity.HasKey (e => e.Idarea)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_area");

                entity.HasIndex (e => e.Clave)
                    .HasName ("IX_gen_area")
                    .IsUnique ();

                entity.HasIndex (e => e.Idarea)
                    .HasName ("ix_gen_area_idarea")
                    .IsUnique ()
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.Idarea).HasColumnName ("idarea");

                entity.Property (e => e.Activa)
                    .IsRequired ()
                    .HasColumnName ("activa")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Clave)
                    .IsRequired ()
                    .HasColumnName ("clave")
                    .HasMaxLength (50)
                    .IsUnicode (false);

                entity.Property (e => e.Clavenivel)
                    .IsRequired ()
                    .HasColumnName ("clavenivel")
                    .HasMaxLength (25)
                    .IsUnicode (false);

                entity.Property (e => e.Descripcion)
                    .IsRequired ()
                    .HasColumnName ("descripcion")
                    .HasMaxLength (255)
                    .IsUnicode (false);

                entity.Property (e => e.Idapertura).HasColumnName ("idapertura");

                entity.Property (e => e.Idunidad).HasColumnName ("idunidad");

                entity.HasOne (d => d.IdaperturaNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idapertura)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_gen_area_gen_aperturaarea");
            });

            modelBuilder.Entity<GenAperturaarea> (entity => {
                entity.HasKey (e => e.Idapertura)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_aperturaarea");

                entity.Property (e => e.Idapertura).HasColumnName ("idapertura");

                entity.Property (e => e.Empresa)
                    .IsRequired ()
                    .HasColumnName ("empresa")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Idmascara).HasColumnName ("idmascara");

                entity.HasOne (d => d.IdmascaraNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idmascara)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_gen_aperturaarea_gen_mascara");
            });

            modelBuilder.Entity<CosPasecentro> (entity => {
                entity.HasKey (e => e.Idpase)
                    .HasName ("PK_cos_PaseCentro")
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_pasecentro");

                entity.Property (e => e.Idpase).HasColumnName ("idpase");

                entity.Property (e => e.Idcentro).HasColumnName ("idcentro");

                entity.Property (e => e.Idregistro).HasColumnName ("idregistro");

                entity.Property (e => e.Importe)
                    .HasColumnName ("importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.HasOne (d => d.IdcentroNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idcentro)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_gasto_cos_centro");

                entity.HasOne (d => d.IdregistroNavigation)
                    .WithMany (p => p.CosPasecentro)
                    .HasPrincipalKey (p => p.Idregistro)
                    .HasForeignKey (d => d.Idregistro)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_pasecentro_cos_registrogasto");
            });

            modelBuilder.Entity<CosRegistrogasto> (entity => {
                entity.HasKey (e => e.Idregistro);

                entity.ToTable ("cos_registrogasto");

                entity.HasIndex (e => e.Idregistro)
                    .HasName ("IX_cos_registrogasto")
                    .IsUnique ();

                entity.Property (e => e.Idregistro).HasColumnName ("idregistro");

                entity.Property (e => e.Importe)
                    .HasColumnName ("importe")
                    .HasColumnType ("numeric(18, 2)");

                entity.Property (e => e.Sumaclave)
                    .HasColumnName ("sumaclave")
                    .HasMaxLength (200)
                    .IsUnicode (false);

                entity.HasOne (d => d.IdregistroNavigation)
                    .WithOne (p => p.CosRegistrogasto)
                    .HasForeignKey<CosRegistrogasto> (d => d.Idregistro)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_registrogasto_con_registroanexo");
            });

            modelBuilder.Entity<ConRegistroanexo> (entity => {
                entity.HasKey (e => e.Idregistro)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("con_registroanexo");

                entity.HasIndex (e => e.Idpase)
                    .HasName ("IX_con_registroanexo")
                    .IsUnique ();

                entity.Property (e => e.Idregistro).HasColumnName ("idregistro");

                entity.Property (e => e.Idoperador).HasColumnName ("idoperador");

                entity.Property (e => e.Idpase).HasColumnName ("idpase");

                entity.HasOne (d => d.IdpaseNavigation)
                    .WithOne ()
                    .HasForeignKey<ConRegistroanexo> (d => d.Idpase)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_con_registroanexo_con_pase");
            });
            modelBuilder.Entity<CosPasesubelemento> (entity => {
                entity.HasKey (e => e.Idpase)
                    .HasName ("PK_cos_Pasesubelemento")
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("cos_pasesubelemento");

                entity.Property (e => e.Idpase)
                    .HasColumnName ("idpase")
                    .ValueGeneratedNever ();

                entity.Property (e => e.Idsubelemento).HasColumnName ("idsubelemento");

                entity.HasOne (d => d.IdpaseNavigation)
                    .WithOne (p => p.CosPasesubelemento)
                    .HasForeignKey<CosPasesubelemento> (d => d.Idpase)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_Pasesubelemento_cos_PaseCentro");

                entity.HasOne (d => d.IdsubelementoNavigation)
                    .WithMany ()
                    .HasForeignKey (d => d.Idsubelemento)
                    .OnDelete (DeleteBehavior.ClientSetNull)
                    .HasConstraintName ("FK_cos_Pasesubelemento_cos_subelementogasto");
            });

            modelBuilder.Entity<GenEntidad> (entity => {
                entity.HasKey (e => e.IdEntidad)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_entidad");

                entity.HasIndex (e => e.IdEntidad)
                    .ForSqlServerIsClustered ();

                entity.Property (e => e.IdEntidad).HasColumnName ("identidad");

                entity.Property (e => e.NIT)
                    .HasColumnName ("NIT");

                entity.Property (e => e.Activo)
                    .IsRequired ()
                    .HasColumnName ("activo")
                    .HasDefaultValueSql ("(1)");

                entity.Property (e => e.Nombre).HasMaxLength (50).HasColumnName ("nombre");

                entity.Property (e => e.Abreviatura).HasMaxLength (50).HasColumnName ("abreviatura");

                entity.Property (e => e.Codigo)
                    .IsRequired ()
                    .HasColumnName ("codigo")
                    .HasMaxLength (50);

                entity.Property (e => e.Email)
                    .HasColumnName ("email ")
                    .HasMaxLength (150);

                entity.Property (e => e.Telefono)
                    .HasColumnName ("telefono ")
                    .HasMaxLength (20);

                entity.Property (e => e.Provincia).HasMaxLength (50);

                entity.Property (e => e.Pais).HasMaxLength (50);

                entity.Property (e => e.Direccion)
                    .HasColumnName ("direccion")
                    .HasMaxLength (150)
                    .HasComputedColumnSql ("(rtrim(isnull([direccion],'')) + ' ' + rtrim(isnull([Provincia],'')) + ' ' + rtrim(isnull([Pais],'')))");

            });
            modelBuilder.Entity<GenCtaBancoEntidad> (entity => {
                entity.HasKey (e => e.NumeroCta)
                    .ForSqlServerIsClustered (false);
                entity.ToTable ("gen_ctabancoentidad");
                entity.HasIndex (e => e.NumeroCta).ForSqlServerIsClustered ();

                entity.Property (e => e.NumeroCta).HasColumnName ("numerocta");

                entity.Property (e => e.IdEntidad).HasColumnName ("identidad");

                entity.Property (e => e.IdMoneda).HasColumnName ("idcuenta");

                entity.Property (e => e.Idsucursal).HasColumnName ("idsucursal");

            });
            modelBuilder.Entity<GenSucursalBanco> (entity => {
                entity.HasKey (e => e.Idsucursal)
                    .ForSqlServerIsClustered (false);

                entity.ToTable ("gen_sucursalbanco");

                entity.HasIndex (e => e.Idsucursal).ForSqlServerIsClustered ();

                entity.Property (e => e.Idsucursal).HasColumnName ("idsucursal");

                entity.Property (e => e.Numero).HasColumnName ("numero");

                entity.Property (e => e.Direccion).HasColumnName ("direccion");

                entity.Property (e => e.Idclasifbanco).HasColumnName ("idclasifbanco");

            });
            modelBuilder.Entity<GenClasifBanco> (entity => {
                entity.HasKey (e => e.Idclasifbanco).ForSqlServerIsClustered (false);

                entity.ToTable ("gen_clasifbanco");

                entity.HasIndex (e => e.Idclasifbanco).ForSqlServerIsClustered ();

                entity.Property (e => e.Idclasifbanco).HasColumnName ("idclasifbanco");

                entity.Property (e => e.Nombre).HasColumnName ("nombre");

                entity.Property (e => e.Codigo).HasColumnName ("codigo");
            });
        }
    }
}