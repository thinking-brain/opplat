using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.Data;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Helper.EstadoFinanciero
{
    public class GetEstadoFinanciero
    {

        FinanzasDbContext _context;
        public GetEstadoFinanciero(FinanzasDbContext context)
        {
            _context = context;
        }

        public List<EstadoFinancieroVM> EstadoFinancieroDetalle(int year, int meses, string efe)
        {
            var resultado = new List<EstadoFinancieroVM>();
            var group = GetEstadoFinancieroDatos.EstadoFinancieroDatos().GroupBy(s => s.Grupo).ToList();

            foreach (var item in group)
            {
                var grupo = new EstadoFinancieroVM { Concepto = item.Key.ToString(), EFE = item.First().EF.ToString(), Detalles = new List<DetalleEstadoFinancieroVM>() };
                foreach (var det in item)
                {
                    string variable = det.Dato.ToString();
                    decimal valor = 0;
                    var cache = _context.Set<CacheEstadoFinanciero>().SingleOrDefault(c => c.Concepto == variable && c.Mes == meses && c.Year == year);
                    if (cache != null)
                    {
                        valor = cache.Saldo;
                    }
                    grupo.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = det.Dato.ToString(), Valor = valor });
                }
                resultado.Add(grupo);
            }
            return resultado;
        }

        public decimal EstadoFinancieroValue(int year, int meses, string efe, string celda)
        {
            var resultado = new List<EstadoFinancieroVM>();
            var elemento = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.EF.Equals(efe) && s.UB.Equals(celda));
            string variable = elemento.Dato.ToString();
            decimal valor = 0;
            var cache = _context.Set<CacheEstadoFinanciero>().SingleOrDefault(c => c.Concepto == variable && c.Mes == meses && c.Year == year);
            if (cache != null)
            {
                valor = cache.Saldo;
            }
            return valor;
        }

        public List<EstadoFinancieroVM> EstadoFinanciero5920(int años, int meses, string efe)
        {
            var data = new List<EstadoFinancieroVM>();
            //Efectivo en Caja  (101-108)
            decimal M11 = EstadoFinancieroValue(años, meses, efe, "M11");
            //Efectivo en Banco y en otras instituciones (109-119)
            decimal M12 = EstadoFinancieroValue(años, meses, efe, "M12");
            //Inversiones a Corto Plazo o Temporales (120-129)
            decimal M13 = EstadoFinancieroValue(años, meses, efe, "M13");
            //Efectos por Cobrar a Corto Plazo (130-133)
            decimal M14 = EstadoFinancieroValue(años, meses, efe, "M14");
            //Menos:Efectos por Cobrar Descontados (365)
            decimal M15 = EstadoFinancieroValue(años, meses, efe, "M15");
            //Cuenta en Participación (134)
            decimal M16 = EstadoFinancieroValue(años, meses, efe, "M16");
            //Cuentas por Cobrar a Corto Plazo (135-139 y 154)
            decimal M17 = EstadoFinancieroValue(años, meses, efe, "M17");
            //Menos: Provisión para Cuentas Incobrables (369)
            decimal M18 = EstadoFinancieroValue(años, meses, efe, "M18");
            //Pagos por Cuentas de Terceros (140) 
            decimal M19 = EstadoFinancieroValue(años, meses, efe, "M19");
            //Participación de Reaseguradores por Siniestros Pend.(141)
            decimal M20 = EstadoFinancieroValue(años, meses, efe, "M20");
            //Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)
            decimal M21 = EstadoFinancieroValue(años, meses, efe, "M21");
            //Suscriptores de Bonos (143) 
            decimal M22 = EstadoFinancieroValue(años, meses, efe, "M22");
            //Pagos Anticipados del Proceso Inversionista (150-152)
            decimal M23 = EstadoFinancieroValue(años, meses, efe, "M23");
            //Materiales Anticipados del Proceso Inversionista (153)
            decimal M24 = EstadoFinancieroValue(años, meses, efe, "M24");
            //Anticipos a Justificar (161-163)
            decimal M25 = EstadoFinancieroValue(años, meses, efe, "M25");
            //Adeudos del Presupuesto del Estado (164-166)
            decimal M26 = EstadoFinancieroValue(años, meses, efe, "M26");
            //Adeudos del Organo u Organismo (167-170)
            decimal M27 = EstadoFinancieroValue(años, meses, efe, "M27");
            //Adeudos del Organo u Organismo (167-170)
            decimal M28 = EstadoFinancieroValue(años, meses, efe, "M28");
            //Ingresos acumulados por Cobrar (173-180)
            decimal M29 = EstadoFinancieroValue(años, meses, efe, "M29");
            //Dividendos y Participaciones opor Cobrar (181) 
            decimal M30 = EstadoFinancieroValue(años, meses, efe, "M30");
            //Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)
            decimal M31 = EstadoFinancieroValue(años, meses, efe, "M31");
            //Materias Primas y Materiales (183)
            decimal M33 = EstadoFinancieroValue(años, meses, efe, "M33");
            //Combustibles y Lubricantes (184)
            decimal M34 = EstadoFinancieroValue(años, meses, efe, "M34");
            //Partes y Piezas de Respuesto (185)
            decimal M35 = EstadoFinancieroValue(años, meses, efe, "M35");
            //Envases y Embalajes (186)
            decimal M36 = EstadoFinancieroValue(años, meses, efe, "M36");
            //Utiles, Herramientas y Otros (187)
            decimal M37 = EstadoFinancieroValue(años, meses, efe, "M37");
            //Menos: Desgaste de Utiles y Herramientas (373)
            decimal M38 = EstadoFinancieroValue(años, meses, efe, "M38");
            //Producción Terminada (188)
            decimal M39 = EstadoFinancieroValue(años, meses, efe, "M39");
            //Mercancias para la Venta (189)
            decimal M40 = EstadoFinancieroValue(años, meses, efe, "M40");
            //Menos: Descuento comercial e Impuesto (370-372)
            decimal M41 = EstadoFinancieroValue(años, meses, efe, "M41");
            //Medicamentos (190)
            decimal M42 = EstadoFinancieroValue(años, meses, efe, "M42");
            //Base Material de Estudio (191)
            decimal M43 = EstadoFinancieroValue(años, meses, efe, "M43");
            //Menos: Desgaste de Base Material de Estudio (366)
            decimal M44 = EstadoFinancieroValue(años, meses, efe, "M44");
            //Vestuario y Lencería (192)
            decimal M54 = EstadoFinancieroValue(años, meses, efe, "M54");
            //Menos: Desgaste de Vestuario y Lenceria  (367)
            decimal M55 = EstadoFinancieroValue(años, meses, efe, "M55");
            //Alimentos (193)
            decimal M56 = EstadoFinancieroValue(años, meses, efe, "M56");
            //Inventario de Mercancías de Importación (194)
            decimal M57 = EstadoFinancieroValue(años, meses, efe, "M57");
            //Inventario de Mercancias de Exportación (195)
            decimal M58 = EstadoFinancieroValue(años, meses, efe, "M58");
            //Producciones para Insumo o Autoconsumo (196)  
            decimal M59 = EstadoFinancieroValue(años, meses, efe, "M59");
            //Otros Inventarios (205-207)
            decimal M60 = EstadoFinancieroValue(años, meses, efe, "M60");
            //Inventarios Ociosos (208)
            decimal M61 = EstadoFinancieroValue(años, meses, efe, "M61");
            //Inventarios de lento Movimiento (209)
            decimal M62 = EstadoFinancieroValue(años, meses, efe, "M62");
            //Producción en Proceso (700-724)
            decimal M63 = EstadoFinancieroValue(años, meses, efe, "M63");
            //Producción Propia para Insumo (725)
            decimal M64 = EstadoFinancieroValue(años, meses, efe, "M64");
            //Reparaciones Capitales con Medios Propios (726)
            decimal M65 = EstadoFinancieroValue(años, meses, efe, "M65");
            //Inversiones con Medios Propios Activos Fijos Intangibles (727)
            decimal M66 = EstadoFinancieroValue(años, meses, efe, "M66");
            //Inversiones con Medios Propios (728)
            decimal M67 = EstadoFinancieroValue(años, meses, efe, "M67");
            //Créditos Documentarios (211)
            decimal M68 = EstadoFinancieroValue(años, meses, efe, "M68");
            //Efectos por Cobrar a Largo Plazo (215-217)
            decimal M70 = EstadoFinancieroValue(años, meses, efe, "M70");
            //Cuentas por Cobrar a Largo Plazo (218-220)
            decimal M71 = EstadoFinancieroValue(años, meses, efe, "M71");
            //Préstamos Concedidos a Cobrar a Largo Plazo (221-224)
            decimal M72 = EstadoFinancieroValue(años, meses, efe, "M72");
            //Inversiones a Largo Plazo o Permanentes (225-234)
            decimal M73 = EstadoFinancieroValue(años, meses, efe, "M73");
            //Activos Fijos Tangibles (240-251)
            decimal M75 = EstadoFinancieroValue(años, meses, efe, "M75");
            //Menos:Depreciación de Activos Fijos Tangibles (375-388)
            decimal M76 = EstadoFinancieroValue(años, meses, efe, "M76");
            //Fondos Bibliotecarios (252)
            decimal M77 = EstadoFinancieroValue(años, meses, efe, "M77");
            //Medios y Equipos para Alquilar  (253)
            decimal M78 = EstadoFinancieroValue(años, meses, efe, "M78");
            //Menos: Desgaste de Medios y Equipos para Alquilar (389)
            decimal M79 = EstadoFinancieroValue(años, meses, efe, "M79");
            //Monumentos y Obras de Arte (254)
            decimal M80 = EstadoFinancieroValue(años, meses, efe, "M80");
            //Activos Fijos Intangibles (255 a 263)
            decimal M81 = EstadoFinancieroValue(años, meses, efe, "M81");
            //Activos Fijos Intangibles en Proceso (264)
            decimal M82 = EstadoFinancieroValue(años, meses, efe, "M82");
            //Menos:Amortización de Activos Fijos Intangibles (390-399)
            decimal M83 = EstadoFinancieroValue(años, meses, efe, "M83");
            //Inversiones en Proceso (265-278)
            decimal M84 = EstadoFinancieroValue(años, meses, efe, "M84");
            //Plan de Preparación de Inversiones (279)
            decimal M85 = EstadoFinancieroValue(años, meses, efe, "M85");
            //Equipos por Instalar y Materiales para el Proceso Inversionista (280-289)
            decimal M86 = EstadoFinancieroValue(años, meses, efe, "M86");
            //Gastos de Produccion y Servicios Diferidos (300-305)
            decimal M88 = EstadoFinancieroValue(años, meses, efe, "M88");
            //Gastos Financieros Diferidos (306-307)
            decimal M89 = EstadoFinancieroValue(años, meses, efe, "M89");
            //Gastos Diferidos del Proceso Inversionista (310-311)
            decimal M90 = EstadoFinancieroValue(años, meses, efe, "M90");
            //Gastos por Faltante y Perdidas Diferidas (312)
            decimal M91 = EstadoFinancieroValue(años, meses, efe, "M91");
            //Pérdidas en Investigación (330-331)
            decimal M93 = EstadoFinancieroValue(años, meses, efe, "M93");
            //Faltante de Bienes en Investigación (332-333)
            decimal M94 = EstadoFinancieroValue(años, meses, efe, "M94");
            //Cuentas por Cobrar Diversas-Operaciones Corrientes (334-341)
            decimal M95 = EstadoFinancieroValue(años, meses, efe, "M95");
            //Cuentas por Cobrar- Compra de Monedas (342)
            decimal M96 = EstadoFinancieroValue(años, meses, efe, "M96");
            //Cuentas por Cobrar Diversa del Proceso Inversionista (343-345)
            decimal M97 = EstadoFinancieroValue(años, meses, efe, "M97");
            //Efectos por Cobrar en Litigio (346)
            decimal M98 = EstadoFinancieroValue(años, meses, efe, "M98");
            //Cuentas por Cobrar en Litigio (347)
            decimal M109 = EstadoFinancieroValue(años, meses, efe, "M109");
            //Efectos por Cobrar Protestados (348)
            decimal M110 = EstadoFinancieroValue(años, meses, efe, "M110");
            //Cuentas por Cobrar en Proceso Judicial (349)
            decimal M111 = EstadoFinancieroValue(años, meses, efe, "M111");
            //Depósitos y Fianzas (354-355)
            decimal M112 = EstadoFinancieroValue(años, meses, efe, "M112");
            //Fondo de Amortización de Bonos - Efectivo en Valores (364)
            decimal M113 = EstadoFinancieroValue(años, meses, efe, "M113");
            //Menos:Otras Provisiones Reguladoras de Activos (374)
            decimal M114 = EstadoFinancieroValue(años, meses, efe, "M114");
            //Sobregiro Bancario (400)
            decimal M118 = EstadoFinancieroValue(años, meses, efe, "M118");
            //Efectos por Pagar a Corto Plazo (401-404)
            decimal M119 = EstadoFinancieroValue(años, meses, efe, "M119");
            //Cuentas por Pagar a Corto Plazo (405-415)
            decimal M120 = EstadoFinancieroValue(años, meses, efe, "M120");
            //Cobros por Cuenta de Terceros (416)
            decimal M121 = EstadoFinancieroValue(años, meses, efe, "M121");
            //Dividendos y Participaciones por Pagar (417)
            decimal M122 = EstadoFinancieroValue(años, meses, efe, "M122");
            //Cuentas en Participación (418-420)
            decimal M123 = EstadoFinancieroValue(años, meses, efe, "M123");
            //Cuentas por Pagar-Activos Fijos Tangibles (421-424)
            decimal M124 = EstadoFinancieroValue(años, meses, efe, "M124");
            //Cuentas por Pagar del Proceso Inversionista (425-429)
            decimal M125 = EstadoFinancieroValue(años, meses, efe, "M125");
            //Cobros Anticipados (430-433)
            decimal M126 = EstadoFinancieroValue(años, meses, efe, "M126");
            //Materiales Recibidos de Forma Anticipa (434)
            decimal M127 = EstadoFinancieroValue(años, meses, efe, "M127");
            //Depósitos Recibidos (435-439)
            decimal M128 = EstadoFinancieroValue(años, meses, efe, "M128");
            //Obligaciones con el Presupuesto del Estado (440-449)
            decimal M129 = EstadoFinancieroValue(años, meses, efe, "M129");
            //Obligaciones con el Organo u Organismo (450-453)
            decimal M130 = EstadoFinancieroValue(años, meses, efe, "M130");
            //Nóminas por Pagar (455-459)
            decimal M131 = EstadoFinancieroValue(años, meses, efe, "M131");
            //Retenciones por Pagar (460-469)
            decimal M132 = EstadoFinancieroValue(años, meses, efe, "M132");
            //Préstamos Recibidos y Otras Operaciones Crediticias por Pagar (470-479)
            decimal M133 = EstadoFinancieroValue(años, meses, efe, "M133");
            //Gastos Acumulados por Pagar (480-489)
            decimal M134 = EstadoFinancieroValue(años, meses, efe, "M134");
            //Provisión para Vacaciones (491;492)
            decimal M135 = EstadoFinancieroValue(años, meses, efe, "M135");
            //Otras Provosiones Operacionales (493-499)
            decimal M136 = EstadoFinancieroValue(años, meses, efe, "M136");
            //Provisión para Pagos de los Subsidios de Seguridad Social a Corto Plazo (500)
            decimal M137 = EstadoFinancieroValue(años, meses, efe, "M137");
            //Fondo de Compensación para Desbalances Financieros (501) uso exclusivo de la OSDE
            decimal M138 = EstadoFinancieroValue(años, meses, efe, "M138");
            //Efectos por Pagar a Largo Plazo (510-514)
            decimal M140 = EstadoFinancieroValue(años, meses, efe, "M140");
            //Cuentas por Pagar a Largo Plazo (515-518)
            decimal M141 = EstadoFinancieroValue(años, meses, efe, "M141");
            //Préstamos Recibidos por Pagar a Largo Plazo (520-524)
            decimal M152 = EstadoFinancieroValue(años, meses, efe, "M152");
            //Obligaciones a Largo Plazo (525-532)
            decimal M153 = EstadoFinancieroValue(años, meses, efe, "M153");
            //Otras Provisiones a Largo Plazo (533-539)
            decimal M154 = EstadoFinancieroValue(años, meses, efe, "M154");
            //Bonos por Pagar (540)-(144)-(363)
            decimal M155 = EstadoFinancieroValue(años, meses, efe, "M155");
            //Bonos Suscritos (541)
            decimal M156 = EstadoFinancieroValue(años, meses, efe, "M156");
            //Ingresos Diferidos (545-548)
            decimal M158 = EstadoFinancieroValue(años, meses, efe, "M158");
            //Ingresos Diferidos por Donaciones Recibidas (549)
            decimal M159 = EstadoFinancieroValue(años, meses, efe, "M159");
            //Sobrantes en Investigación (555-564)
            decimal M161 = EstadoFinancieroValue(años, meses, efe, "M161");
            //Cuentas por Pagar Diversas (565-568)
            decimal M162 = EstadoFinancieroValue(años, meses, efe, "M162");
            //Cuentas por Pagar- Compra de Moneda (569)
            decimal M163 = EstadoFinancieroValue(años, meses, efe, "M163");
            //Ingresos de Períodos Futuros (570-574)
            decimal M164 = EstadoFinancieroValue(años, meses, efe, "M164");
            //Obligaciones con el Presupuesto del Estado por Garantía Activada (575)
            decimal M165 = EstadoFinancieroValue(años, meses, efe, "M165");
            //Inversión Estatal (600-612) Sector Público
            decimal M169 = EstadoFinancieroValue(años, meses, efe, "M169");
            //Patrimonio y Fondo Común (600) Sector Privado
            decimal M170 = EstadoFinancieroValue(años, meses, efe, "M170");
            //Recursos Recibidos  (617-618) Sector Publico
            decimal M172 = EstadoFinancieroValue(años, meses, efe, "M172");
            //Donaciones Recibidas - Nacionales (620)
            decimal M173 = EstadoFinancieroValue(años, meses, efe, "M173");
            //Donaciones Recibidas - Exterior (621)
            decimal M174 = EstadoFinancieroValue(años, meses, efe, "M174");
            //Utilidad  Retenida (630-634)
            decimal M175 = EstadoFinancieroValue(años, meses, efe, "M175");
            //Subvención por Pérdida  (635-639)
            decimal M176 = EstadoFinancieroValue(años, meses, efe, "M176");
            //Reservas para Contingencias (645)
            decimal M177 = EstadoFinancieroValue(años, meses, efe, "M177");
            //Otras Reservas Patrimoniales (646-654)
            decimal M178 = EstadoFinancieroValue(años, meses, efe, "M178");
            //Fondo de Contravalor para proyectos de Inversión (688)
            decimal M179 = EstadoFinancieroValue(años, meses, efe, "M179");
            //Menos: Recursos Entregados (619) Sector Publico
            decimal M180 = EstadoFinancieroValue(años, meses, efe, "M180");
            // Donaciones Entregadas - Nacionales (626)
            decimal M181 = EstadoFinancieroValue(años, meses, efe, "M181");
            //Donaciones Entregadas - Exterior (627)
            decimal M182 = EstadoFinancieroValue(años, meses, efe, "M182");
            //Pago a Cuenta de Utilidades (690)
            decimal M183 = EstadoFinancieroValue(años, meses, efe, "M183");
            //Pago a Cuenta de Dividendos (691)
            decimal M184 = EstadoFinancieroValue(años, meses, efe, "M184");
            //Pérdida (640-644)
            decimal M185 = EstadoFinancieroValue(años, meses, efe, "M185");
            //Mas o Menos: Reevalucion de Activos Fijos Tangibles (613-615)
            decimal M186 = EstadoFinancieroValue(años, meses, efe, "M186");
            //Otras Operaciones de Capital (616-619) Sector Privado
            decimal M187 = EstadoFinancieroValue(años, meses, efe, "M187");
            //Revaluación de Inventarios (697)
            decimal M188 = EstadoFinancieroValue(años, meses, efe, "M188");
            //Ganancia o Pérdida no Realizada (698)
            decimal M189 = EstadoFinancieroValue(años, meses, efe, "M189");

            decimal M32 = ((M33 + M34 + M35 + M36 + M37 + M39 + M40 + M42 + M43 + M54) + (M56 + M57 + M58 + M59 + M60 + M61 + M62 + M63 + M64 + M65 + M66 + M67)) - (M38 + M41 + M44 + M55);
            var ActivoCirculante = (M16 + M17 + M11 + M12 + M13 + M14) + (M19 + M20 + M21 + M22 + M23 + M24 + M25 + M26 + M27 + M28 + M29 + M30 + M31 + M32 + M68) - M15 - M18;
            decimal M10 = ActivoCirculante;
            var TotalInventario = M32;
            var ActivosALargoPlazo = (M70 + M71 + M72 + M73);
            decimal M69 = ActivosALargoPlazo;
            var ActivosFijos = (M78 + M77 + M75 + M80 + M81 + M82 + M84 + M85 + M86) - (M76 + M79 + M83);
            decimal M74 = ActivosFijos;
            var ActivosDiferidos = (M88 + M89 + M90 + M91);
            decimal M87 = ActivosDiferidos;
            var OtrosActivos = (M93 + M94 + M95 + M96 + M97 + M98 + M109 + M110 + M111 + M112 + M113) - M114;
            decimal M92 = OtrosActivos;
            var TotalDeActivos = (M10 + M69 + M74 + M87 + M92);
            decimal M115 = TotalDeActivos;
            var PasivoCirculante = (M118 + M119 + M120 + M121 + M122 + M123 + M124 + M125 + M126 + M127 + M128 + M129 + M130 + M131 + M132 + M133 + M134 + M135 + M136 + M137 + M138);
            decimal M117 = PasivoCirculante;
            var PasivosLargoPlazo = M140 + M141 + (M152 + M153 + M154 + M155 + M156);
            decimal M139 = PasivosLargoPlazo;
            var PasivosDiferidos = (M158 + M159);
            decimal M157 = PasivosDiferidos;
            var OtrosPasivos = (M161 + M162 + M163 + M164 + M165);
            decimal M160 = OtrosPasivos;
            var TotalDePasivos = (M117 + M139 + M157 + M160);
            decimal M166 = TotalDePasivos;
            var TotalPatrimonioNeto = (M169 + M170 + M172 + M173 + M174 + M175 + M176 + M177 + M178 + M179) - (M180 + M181 + M182 + M183 + M184 + M185) + M186 + M187 + M188 + M189;
            var M200 = TotalPatrimonioNeto;
            var TotalPasivoPatrimonioNetoCapitalContable = (M166 + M200);
            //ACTIVOS CIRCULANTES
            var ac = new EstadoFinancieroVM { Concepto = "Activo Circulante", Valor = ActivoCirculante, Detalles = new List<DetalleEstadoFinancieroVM>() };
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M11") && s.EF == efe).Dato.ToString(), Valor = M11 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M12") && s.EF == efe).Dato.ToString(), Valor = M12 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M13") && s.EF == efe).Dato.ToString(), Valor = M13 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M14") && s.EF == efe).Dato.ToString(), Valor = M14 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M15") && s.EF == efe).Dato.ToString(), Valor = M15 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M16") && s.EF == efe).Dato.ToString(), Valor = M16 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M17") && s.EF == efe).Dato.ToString(), Valor = M17 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M18") && s.EF == efe).Dato.ToString(), Valor = M18 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M19") && s.EF == efe).Dato.ToString(), Valor = M19 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M20") && s.EF == efe).Dato.ToString(), Valor = M20 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M21") && s.EF == efe).Dato.ToString(), Valor = M21 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M22") && s.EF == efe).Dato.ToString(), Valor = M22 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M23") && s.EF == efe).Dato.ToString(), Valor = M23 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M24") && s.EF == efe).Dato.ToString(), Valor = M24 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M25") && s.EF == efe).Dato.ToString(), Valor = M25 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M26") && s.EF == efe).Dato.ToString(), Valor = M26 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M27") && s.EF == efe).Dato.ToString(), Valor = M27 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M28") && s.EF == efe).Dato.ToString(), Valor = M28 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M29") && s.EF == efe).Dato.ToString(), Valor = M29 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M30") && s.EF == efe).Dato.ToString(), Valor = M30 });
            ac.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M31") && s.EF == efe).Dato.ToString(), Valor = M31 });
            data.Add(ac);
            //TOTAL DE INVENTARIO
            var ti = new EstadoFinancieroVM { Concepto = "Total de Inventario", Valor = TotalInventario, Detalles = new List<DetalleEstadoFinancieroVM>() };
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M33") && s.EF == efe).Dato.ToString(), Valor = M33 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M34") && s.EF == efe).Dato.ToString(), Valor = M34 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M35") && s.EF == efe).Dato.ToString(), Valor = M35 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M36") && s.EF == efe).Dato.ToString(), Valor = M36 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M37") && s.EF == efe).Dato.ToString(), Valor = M37 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M38") && s.EF == efe).Dato.ToString(), Valor = M38 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M39") && s.EF == efe).Dato.ToString(), Valor = M39 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M40") && s.EF == efe).Dato.ToString(), Valor = M40 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M41") && s.EF == efe).Dato.ToString(), Valor = M41 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M42") && s.EF == efe).Dato.ToString(), Valor = M42 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M43") && s.EF == efe).Dato.ToString(), Valor = M43 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M44") && s.EF == efe).Dato.ToString(), Valor = M44 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M54") && s.EF == efe).Dato.ToString(), Valor = M54 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M55") && s.EF == efe).Dato.ToString(), Valor = M55 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M56") && s.EF == efe).Dato.ToString(), Valor = M56 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M57") && s.EF == efe).Dato.ToString(), Valor = M57 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M58") && s.EF == efe).Dato.ToString(), Valor = M58 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M59") && s.EF == efe).Dato.ToString(), Valor = M59 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M60") && s.EF == efe).Dato.ToString(), Valor = M60 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M61") && s.EF == efe).Dato.ToString(), Valor = M61 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M62") && s.EF == efe).Dato.ToString(), Valor = M62 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M63") && s.EF == efe).Dato.ToString(), Valor = M63 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M64") && s.EF == efe).Dato.ToString(), Valor = M64 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M65") && s.EF == efe).Dato.ToString(), Valor = M65 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M66") && s.EF == efe).Dato.ToString(), Valor = M66 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M67") && s.EF == efe).Dato.ToString(), Valor = M67 });
            ti.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M68") && s.EF == efe).Dato.ToString(), Valor = M68 });
            data.Add(ti);
            //ACTIVOS A LARGO PLAZO
            var apl = new EstadoFinancieroVM { Concepto = "Activos a Largo Plazo", Valor = ActivosALargoPlazo, Detalles = new List<DetalleEstadoFinancieroVM>() };
            apl.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M70") && s.EF == efe).Dato.ToString(), Valor = M70 });
            apl.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M71") && s.EF == efe).Dato.ToString(), Valor = M71 });
            apl.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M72") && s.EF == efe).Dato.ToString(), Valor = M72 });
            apl.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M73") && s.EF == efe).Dato.ToString(), Valor = M73 });
            data.Add(apl);
            //ACTIVOS FIJOS
            var af = new EstadoFinancieroVM { Concepto = "Activos Fijos", Valor = ActivosFijos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M75") && s.EF == efe).Dato.ToString(), Valor = M75 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M76") && s.EF == efe).Dato.ToString(), Valor = M76 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M77") && s.EF == efe).Dato.ToString(), Valor = M77 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M78") && s.EF == efe).Dato.ToString(), Valor = M78 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M79") && s.EF == efe).Dato.ToString(), Valor = M79 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M80") && s.EF == efe).Dato.ToString(), Valor = M80 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M81") && s.EF == efe).Dato.ToString(), Valor = M81 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M82") && s.EF == efe).Dato.ToString(), Valor = M82 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M83") && s.EF == efe).Dato.ToString(), Valor = M83 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M84") && s.EF == efe).Dato.ToString(), Valor = M84 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M85") && s.EF == efe).Dato.ToString(), Valor = M85 });
            af.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M86") && s.EF == efe).Dato.ToString(), Valor = M86 });
            data.Add(af);
            //ACTIVOS DIFERIDOS
            var ad = new EstadoFinancieroVM { Concepto = "Activos Diferidos", Valor = ActivosDiferidos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            ad.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M88") && s.EF == efe).Dato.ToString(), Valor = M88 });
            ad.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M89") && s.EF == efe).Dato.ToString(), Valor = M89 });
            ad.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M90") && s.EF == efe).Dato.ToString(), Valor = M90 });
            ad.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M91") && s.EF == efe).Dato.ToString(), Valor = M91 });
            data.Add(ad);
            //OTROS DIFERIDOS
            var oa = new EstadoFinancieroVM { Concepto = "Otros Activos", Valor = OtrosActivos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M93") && s.EF == efe).Dato.ToString(), Valor = M93 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M94") && s.EF == efe).Dato.ToString(), Valor = M94 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M95") && s.EF == efe).Dato.ToString(), Valor = M95 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M96") && s.EF == efe).Dato.ToString(), Valor = M96 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M97") && s.EF == efe).Dato.ToString(), Valor = M97 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M98") && s.EF == efe).Dato.ToString(), Valor = M98 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M109") && s.EF == efe).Dato.ToString(), Valor = M109 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M110") && s.EF == efe).Dato.ToString(), Valor = M110 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M111") && s.EF == efe).Dato.ToString(), Valor = M111 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M112") && s.EF == efe).Dato.ToString(), Valor = M112 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M113") && s.EF == efe).Dato.ToString(), Valor = M113 });
            oa.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M114") && s.EF == efe).Dato.ToString(), Valor = M114 });
            data.Add(oa);
            //TOTAL DE ACTIVOS
            var ta = new EstadoFinancieroVM { Concepto = "Total de Activos", Valor = TotalDeActivos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            data.Add(ta);
            //PASIVO CIRCULANTE
            var pc = new EstadoFinancieroVM { Concepto = "Pasivo Circulante", Valor = PasivoCirculante, Detalles = new List<DetalleEstadoFinancieroVM>() };
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M118") && s.EF == efe).Dato.ToString(), Valor = M118 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M119") && s.EF == efe).Dato.ToString(), Valor = M119 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M120") && s.EF == efe).Dato.ToString(), Valor = M120 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M121") && s.EF == efe).Dato.ToString(), Valor = M121 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M122") && s.EF == efe).Dato.ToString(), Valor = M122 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M123") && s.EF == efe).Dato.ToString(), Valor = M123 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M124") && s.EF == efe).Dato.ToString(), Valor = M124 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M125") && s.EF == efe).Dato.ToString(), Valor = M125 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M126") && s.EF == efe).Dato.ToString(), Valor = M126 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M127") && s.EF == efe).Dato.ToString(), Valor = M127 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M128") && s.EF == efe).Dato.ToString(), Valor = M128 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M129") && s.EF == efe).Dato.ToString(), Valor = M129 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M130") && s.EF == efe).Dato.ToString(), Valor = M130 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M131") && s.EF == efe).Dato.ToString(), Valor = M131 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M132") && s.EF == efe).Dato.ToString(), Valor = M132 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M133") && s.EF == efe).Dato.ToString(), Valor = M133 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M134") && s.EF == efe).Dato.ToString(), Valor = M134 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M135") && s.EF == efe).Dato.ToString(), Valor = M135 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M136") && s.EF == efe).Dato.ToString(), Valor = M136 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M137") && s.EF == efe).Dato.ToString(), Valor = M137 });
            pc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M138") && s.EF == efe).Dato.ToString(), Valor = M138 });
            data.Add(pc);
            //PASIVOS A LARGO PLAZO
            var plp = new EstadoFinancieroVM { Concepto = "Pasivos a Largo Plazo", Valor = PasivosLargoPlazo, Detalles = new List<DetalleEstadoFinancieroVM>() };
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M140") && s.EF == efe).Dato.ToString(), Valor = M140 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M141") && s.EF == efe).Dato.ToString(), Valor = M141 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M152") && s.EF == efe).Dato.ToString(), Valor = M152 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M153") && s.EF == efe).Dato.ToString(), Valor = M153 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M154") && s.EF == efe).Dato.ToString(), Valor = M154 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M155") && s.EF == efe).Dato.ToString(), Valor = M155 });
            plp.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M156") && s.EF == efe).Dato.ToString(), Valor = M156 });
            data.Add(plp);
            //PASIVOS DIFERIDOS
            var pd = new EstadoFinancieroVM { Concepto = "Pasivos Diferidos", Valor = PasivosLargoPlazo, Detalles = new List<DetalleEstadoFinancieroVM>() };
            pd.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M158") && s.EF == efe).Dato.ToString(), Valor = M158 });
            pd.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M159") && s.EF == efe).Dato.ToString(), Valor = M159 });
            data.Add(pd);
            //OTROS PASIVOS
            var op = new EstadoFinancieroVM { Concepto = "Otros Pasivos", Valor = OtrosPasivos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            op.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M161") && s.EF == efe).Dato.ToString(), Valor = M161 });
            op.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M162") && s.EF == efe).Dato.ToString(), Valor = M162 });
            op.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M163") && s.EF == efe).Dato.ToString(), Valor = M163 });
            op.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M164") && s.EF == efe).Dato.ToString(), Valor = M164 });
            op.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M165") && s.EF == efe).Dato.ToString(), Valor = M165 });
            data.Add(op);
            //TOTAL DE PASIVOS
            var tp = new EstadoFinancieroVM { Concepto = "Total Pasivos", Valor = TotalDePasivos, Detalles = new List<DetalleEstadoFinancieroVM>() };
            data.Add(tp);
            //PATRIMONIO NETO
            var pnocc = new EstadoFinancieroVM { Concepto = "PATRIMONIO NETO O CAPITAL CONTABLE", Valor = 0, Detalles = new List<DetalleEstadoFinancieroVM>() };
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M169") && s.EF == efe).Dato.ToString(), Valor = M169 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M170") && s.EF == efe).Dato.ToString(), Valor = M170 });
            // pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M171") && s.EF == efe).Dato.ToString(), Valor = M171 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M172") && s.EF == efe).Dato.ToString(), Valor = M172 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M173") && s.EF == efe).Dato.ToString(), Valor = M173 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M174") && s.EF == efe).Dato.ToString(), Valor = M174 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M175") && s.EF == efe).Dato.ToString(), Valor = M175 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M176") && s.EF == efe).Dato.ToString(), Valor = M176 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M177") && s.EF == efe).Dato.ToString(), Valor = M177 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M178") && s.EF == efe).Dato.ToString(), Valor = M178 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M179") && s.EF == efe).Dato.ToString(), Valor = M179 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M180") && s.EF == efe).Dato.ToString(), Valor = M180 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M181") && s.EF == efe).Dato.ToString(), Valor = M181 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M182") && s.EF == efe).Dato.ToString(), Valor = M182 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M183") && s.EF == efe).Dato.ToString(), Valor = M183 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M184") && s.EF == efe).Dato.ToString(), Valor = M184 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M185") && s.EF == efe).Dato.ToString(), Valor = M185 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M186") && s.EF == efe).Dato.ToString(), Valor = M186 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M187") && s.EF == efe).Dato.ToString(), Valor = M187 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M188") && s.EF == efe).Dato.ToString(), Valor = M188 });
            pnocc.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.UB.Equals("M189") && s.EF == efe).Dato.ToString(), Valor = M189 });
            data.Add(pnocc);
            //TOTAL PATRIMONIO NETO
            var tpn = new EstadoFinancieroVM { Concepto = "TOTAL PATRIMONIO NETO", Valor = TotalPatrimonioNeto, Detalles = new List<DetalleEstadoFinancieroVM>() };
            data.Add(tpn);
            //TOTAL DEL PASIVO Y PATRIMONIO NETO
            var tppncc = new EstadoFinancieroVM { Concepto = "TOTAL DEL PASIVO Y PATRIMONIO NETO O CAPITAL CONTABLE", Valor = TotalPasivoPatrimonioNetoCapitalContable, Detalles = new List<DetalleEstadoFinancieroVM>() };
            data.Add(tppncc);
            return data;
        }





    }
}
