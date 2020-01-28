using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.Data;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Helper.EstadoFinanciero
{
    public class GetEstadoFinancieroDatos
    {

        FinanzasDbContext _context;
        public GetEstadoFinancieroDatos(FinanzasDbContext context)
        {
            _context = context;
        }

        public static List<dynamic> EstadoFinancieroDatos()
        {
            var data = new List<dynamic> {
                ////////////////////////////////////////5920/////////////////////////////////////////////
                //Activo Circulante

                ///ARREGLAR
                new { EF = "5920", UB="M11", Grupo = "Activo Circulante", Dato ="Efectivo en Caja (101-108)", Valor = "101,102,103,104,105,106,107,108"},
                new { EF = "5920", UB="M12", Grupo = "Activo Circulante", Dato ="Efectivo en Banco y en otras instituciones (109-119)", Valor = "109,110,111,112,113,114,115,116,117,118,119"},
                new { EF = "5920", UB="M13", Grupo = "Activo Circulante", Dato ="Inversiones a Corto Plazo o Temporales (120-129)", Valor = "120,121,122,123,124,125,126,127,128,129"},
                new { EF = "5920", UB="M14", Grupo = "Activo Circulante", Dato ="Efectos por Cobrar a Corto Plazo (130-133)", Valor = "130,131,132,133"},
                new { EF = "5920", UB="M15", Grupo = "Activo Circulante", Dato ="Menos:Efectos por Cobrar Descontados (365-368)", Valor = "365,366,367,368"},
                new { EF = "5920", UB="M16", Grupo = "Activo Circulante", Dato ="Cuenta en Participación (134)", Valor = "134"},
                new { EF = "5920", UB="M17", Grupo = "Activo Circulante", Dato ="Cuentas por Cobrar a Corto Plazo (135-139 y 154)", Valor = "135,136,137,138,139,154"},
                new { EF = "5920", UB="M18", Grupo = "Activo Circulante", Dato ="Menos: Provisión para Cuentas Incobrables (369)", Valor = "369"},
                new { EF = "5920", UB="M19", Grupo = "Activo Circulante", Dato ="Pagos por Cuentas de Terceros (140)", Valor = "140"},
                new { EF = "5920", UB="M20", Grupo = "Activo Circulante", Dato ="Participación de Reaseguradoras por Siniestros Pendientes (141)", Valor = "141"},
                new { EF = "5920", UB="M21", Grupo = "Activo Circulante", Dato ="Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)", Valor = "142"},
                new { EF = "5920", UB="M22", Grupo = "Activo Circulante", Dato ="Suscriptores de Bonos (143)", Valor = "143"},
                new { EF = "5920", UB="M23", Grupo = "Activo Circulante", Dato ="Pagos Anticipados a Suministradores (146-149)", Valor = "146,147,148,149"},
                new { EF = "5920", UB="M24", Grupo = "Activo Circulante", Dato ="Pagos Anticipados del Proceso Inversionista (150-153)", Valor = "150,151,152,153"},
                new { EF = "5920", UB="M25", Grupo = "Activo Circulante", Dato ="Materiales Anticipados del Proceso Inversionista (153)", Valor = "153"},
                new { EF = "5920", UB="M26", Grupo = "Activo Circulante", Dato ="Anticipos a Justificar (161-163)", Valor = "161,162,163"},
                new { EF = "5920", UB="M27", Grupo = "Activo Circulante", Dato ="Adeudos del Presupuesto del Estado (164-166)", Valor = "164,165,166"},
                new { EF = "5920", UB="M28", Grupo = "Activo Circulante", Dato ="Adeudos del Organo u Organismo (167-170)", Valor = "167,168,169,170"},
                new { EF = "5920", UB="M29", Grupo = "Activo Circulante", Dato ="Ingresos acumulados por Cobrar (173-180)", Valor = "173,174,175,176,177,178,179,180"},
                new { EF = "5920", UB="M30", Grupo = "Activo Circulante", Dato ="Dividendos y Participaciones o por Cobrar (181)", Valor = "181"},
                new { EF = "5920", UB="M31", Grupo = "Activo Circulante", Dato ="Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)", Valor = "182"},
                //Total de Inventario
                new { EF = "5920", UB="M33", Grupo = "Total de Inventario", Dato = "Materias Primas y Materiales (183)", Valor = "183"},
                new { EF = "5920", UB="M34", Grupo = "Total de Inventario", Dato ="Combustibles y Lubricantes (184)", Valor = "184"},
                new { EF = "5920", UB="M35", Grupo = "Total de Inventario", Dato ="Partes y Piezas de Respuesto (185)", Valor = "185"},
                new { EF = "5920", UB="M36", Grupo = "Total de Inventario", Dato ="Envases y Embalajes (186)", Valor = "186"},
                new { EF = "5920", UB="M37", Grupo = "Total de Inventario", Dato ="Utiles, Herramientas y Otros (187)", Valor = "187"},
                new { EF = "5920", UB="M38", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Utiles y Herramientas (373)", Valor = "373"},
                new { EF = "5920", UB="M39", Grupo = "Total de Inventario", Dato ="Producción Terminada (188)", Valor = "188"},
                new { EF = "5920", UB="M40", Grupo = "Total de Inventario", Dato ="Mercancias para la Venta (189)", Valor = "189"},
                new { EF = "5920", UB="M41", Grupo = "Total de Inventario", Dato ="Menos: Descuento comercial e Impuesto (370-372)", Valor = "370,371,372"},
                new { EF = "5920", UB="M42", Grupo = "Total de Inventario", Dato ="Medicamentos (190)", Valor = "190"},
                new { EF = "5920", UB="M43", Grupo = "Total de Inventario", Dato ="Base Material de Estudio (191)", Valor = "191"},
                new { EF = "5920", UB="M44", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Base Material de Estudio (366)", Valor = "366"},
                new { EF = "5920", UB="M54", Grupo = "Total de Inventario", Dato ="Vestuario y Lencería (192)", Valor = "192"},
                new { EF = "5920", UB="M55", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Vestuario y Lenceria (367)", Valor = "367"},
                new { EF = "5920", UB="M56", Grupo = "Total de Inventario", Dato ="Alimentos (193)", Valor = "193"},
                new { EF = "5920", UB="M57", Grupo = "Total de Inventario", Dato ="Inventario de Mercancías de Importación (194)", Valor = "194"},
                new { EF = "5920", UB="M58", Grupo = "Total de Inventario", Dato ="Inventario de Mercancias de Exportación (195)", Valor = "195"},
                new { EF = "5920", UB="M59", Grupo = "Total de Inventario", Dato ="Producciones para Insumo o Autoconsumo (196)", Valor = "196"},
                new { EF = "5920", UB="M60", Grupo = "Total de Inventario", Dato ="Otros Inventarios (205-207)", Valor = "205,206,207"},
                new { EF = "5920", UB="M61", Grupo = "Total de Inventario", Dato ="Inventarios Ociosos (208)", Valor = "208"},
                new { EF = "5920", UB="M62", Grupo = "Total de Inventario", Dato ="Inventarios de lento Movimiento (209)", Valor = "209"},
                new { EF = "5920", UB="M63", Grupo = "Total de Inventario", Dato ="Producción en Proceso (700-724)", Valor = "700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717,718,719,720,721,722,723,724"},
                new { EF = "5920", UB="M64", Grupo = "Total de Inventario", Dato ="Produccion Propia para Insumo o Autoconsumo (725)", Valor = "725"},
                new { EF = "5920", UB="M65", Grupo = "Total de Inventario", Dato ="Reparaciones Capitales con Medios Propios (726)", Valor = "726"},
                new { EF = "5920", UB="M66", Grupo = "Total de Inventario", Dato ="Inversiones con Medios Propios Activos Fijos Intangibles (727)", Valor = "727"},
                new { EF = "5920", UB="M67", Grupo = "Total de Inventario", Dato ="Inversiones con Medios Propios (728)", Valor = "728"},
                new { EF = "5920", UB="M68", Grupo = "Total de Inventario", Dato ="Créditos Documentarios (211)", Valor = "211"},
                //Activos a Largo Plazo
                new { EF = "5920", UB="M70", Grupo = "Activos a Largo Plazo", Dato = "Efectos por Cobrar a Largo Plazo (215-217)", Valor = "215,216,217"},
                new { EF = "5920", UB="M71", Grupo = "Activos a Largo Plazo", Dato ="Cuentas por Cobrar a Largo Plazo (218-220)", Valor = "218,219,220"},
                new { EF = "5920", UB="M72", Grupo = "Activos a Largo Plazo", Dato ="Préstamos Concedidos a Cobrar a Largo Plazo (221-224)", Valor = "221,222,223,224"},
                new { EF = "5920", UB="M73", Grupo = "Activos a Largo Plazo", Dato ="Cuentas en Participación (418-420)", Valor = "418,419,420"},
                //Activos Fijos	
                new { EF = "5920", UB="M75", Grupo = "Activos Fijos", Dato = "Activos Fijos Tangibles (240-251)", Valor = "240,241,242,243,244,245,246,247,248,249,250,251"},
                new { EF = "5920", UB="M76", Grupo = "Activos Fijos", Dato = "Menos:Depreciación de Activos Fijos Tangibles (375-388)", Valor = "375,376,377,378,379,380,381,382,383,384,385,386,387,388"},
                new { EF = "5920", UB="M77", Grupo = "Activos Fijos", Dato = "Fondos Bibliotecarios (252)", Valor = "252"},
                new { EF = "5920", UB="M78", Grupo = "Activos Fijos", Dato = "Medios y Equipos para Alquilar (253)", Valor = "253"},
                new { EF = "5920", UB="M79", Grupo = "Activos Fijos", Dato = "Menos: Desgaste de Medios y Equipos para Alquilar (389)", Valor = "289"},
                new { EF = "5920", UB="M80", Grupo = "Activos Fijos", Dato = "Monumentos y Obras de Arte (254)", Valor = "254"},
                new { EF = "5920", UB="M81", Grupo = "Activos Fijos", Dato = "Activos Fijos Intangibles (255 a 263)", Valor = "255,256,257,258,259,260,261,262,263"},
                new { EF = "5920", UB="M82", Grupo = "Activos Fijos", Dato = "Activos Fijos Intangibles en Proceso (264)", Valor = "264"},
                new { EF = "5920", UB="M83", Grupo = "Activos Fijos", Dato = "Menos:Amortización de Activos Fijos Intangibles (390-399)", Valor = "390,391,392,393,394,395,396,397,398,399"},
                new { EF = "5920", UB="M84", Grupo = "Activos Fijos", Dato = "Inversiones en Proceso (265-278)", Valor = "265,266,267,268,269,270,271,272,273,274,275,276,277,278"},
                new { EF = "5920", UB="M85", Grupo = "Activos Fijos", Dato = "Plan de Preparación de Inversiones (279)", Valor = "279"},
                new { EF = "5920", UB="M86", Grupo = "Activos Fijos", Dato = "Equipos por Instalar y Materiales para el Proceso Inversionista (280-289)", Valor = "280,281,282,283,284,285,286,287,288,289"},				
            	//Activos Diferidos	
                new { EF = "5920", UB="M88", Grupo = "Activos Diferidos", Dato = "Gastos de Produccion y Servicios Diferidos (300-305)", Valor = "300,301,302,303,304,305"},
                new { EF = "5920", UB="M89", Grupo = "Activos Diferidos", Dato = "Gastos Financieros Diferidos (306-307)", Valor = "306,307"},
                new { EF = "5920", UB="M90", Grupo = "Activos Diferidos", Dato = "Gastos Diferidos del Proceso Inversionista (310-311)", Valor = "310,311"},
                new { EF = "5920", UB="M91", Grupo = "Activos Diferidos", Dato = "Gastos por Faltante y Perdidas Diferidas (312)", Valor = "312"},								
                //Otros Activos
                new { EF = "5920", UB="M93", Grupo = "Otros Activos", Dato = "Pérdidas en Investigación (330-331)", Valor = "330,331"},
                new { EF = "5920", UB="M94", Grupo = "Otros Activos", Dato = "Faltante de Bienes en Investigación (332-333)", Valor = "332,333"},
                new { EF = "5920", UB="M95", Grupo = "Otros Activos", Dato = "Cuentas por Cobrar Diversas-Operaciones Corrientes (334-341)", Valor = "334,335,336,337,338,339,340,341"},
                new { EF = "5920", UB="M96", Grupo = "Otros Activos", Dato = "Cuentas por Cobrar- Compra de Monedas (342)", Valor = "342"},
                new { EF = "5920", UB="M97", Grupo = "Otros Activos", Dato = "Cuentas por Cobrar Diversa del Proceso Inversionista (343-345)", Valor = "343,344,345"},
                new { EF = "5920", UB="M98", Grupo = "Otros Activos", Dato = "Efectos por Cobrar en Litigio (346)", Valor = "346"},
                new { EF = "5920", UB="M109", Grupo = "Otros Activos", Dato = "Cuentas por Cobrar en Litigio (347)", Valor = "347"},
                new { EF = "5920", UB="M110", Grupo = "Otros Activos", Dato = "Efectos por Cobrar Protestados (348)", Valor = "348"},
                new { EF = "5920", UB="M111", Grupo = "Otros Activos", Dato = "Cuentas por Cobrar en Proceso Judicial (349)", Valor = "349"},
                new { EF = "5920", UB="M112", Grupo = "Otros Activos", Dato = "Depósitos y Fianzas (354-355)", Valor = "354,355"},
                new { EF = "5920", UB="M113", Grupo = "Otros Activos", Dato = "Fondo de Amortización de Bonos - Efectivo en Valores (364)", Valor = "364"},
                new { EF = "5920", UB="M114", Grupo = "Otros Activos", Dato = "Menos:Otras Provisiones Reguladoras de Activos (374)", Valor = "374"},
			    //Pasivo Circulante
                new { EF = "5920", UB="M118", Grupo = "Pasivo Circulante", Dato = "Sobregiro Bancario (400)", Valor = "400"},
                new { EF = "5920", UB="M119", Grupo = "Pasivo Circulante", Dato = "Efectos por Pagar a Corto Plazo (401-404)", Valor = "401,402,403,404"},
                new { EF = "5920", UB="M120", Grupo = "Pasivo Circulante", Dato = "Cuentas por Pagar a Corto Plazo (405-415)", Valor = "405,406,407,408,409,410,411,412,413,414,415"},
                new { EF = "5920", UB="M121", Grupo = "Pasivo Circulante", Dato = "Cobros por Cuenta de Terceros (416)", Valor = "416"},
                new { EF = "5920", UB="M122", Grupo = "Pasivo Circulante", Dato = "Dividendos y Participaciones por Pagar (417)", Valor = "417"},
                new { EF = "5920", UB="M123", Grupo = "Pasivo Circulante", Dato = "Cuentas en Participación (418-420)", Valor = "418,419,420"},
                new { EF = "5920", UB="M124", Grupo = "Pasivo Circulante", Dato = "Cuentas por Pagar-Activos Fijos Tangibles (421-424)", Valor = "421,422,423,424"},
                new { EF = "5920", UB="M125", Grupo = "Pasivo Circulante", Dato = "Cuentas por Pagar del Proceso Inversionista (425-429)", Valor = "425,426,427,428,429"},
                new { EF = "5920", UB="M126", Grupo = "Pasivo Circulante", Dato = "Cobros Anticipados (430-433)", Valor = "430,431,432,433"},
                new { EF = "5920", UB="M127", Grupo = "Pasivo Circulante", Dato = "Materiales Recibidos de Forma Anticipa (434)", Valor = "434"},
                new { EF = "5920", UB="M128", Grupo = "Pasivo Circulante", Dato = "Depósitos Recibidos (435-439)", Valor = "435,436,437,438,439"},
                new { EF = "5920", UB="M129", Grupo = "Pasivo Circulante", Dato = "Obligaciones con el Presupuesto del Estado (440-449)", Valor = "440,441,442,443,445,446,447,448,449"},
                new { EF = "5920", UB="M130", Grupo = "Pasivo Circulante", Dato = "Obligaciones con el Organo u Organismo (450-453)", Valor = "450,451,452,453"},
                new { EF = "5920", UB="M131", Grupo = "Pasivo Circulante", Dato = "Nóminas por Pagar (455-459)", Valor = "455,456,457,458,459"},
                new { EF = "5920", UB="M132", Grupo = "Pasivo Circulante", Dato = "Retenciones por Pagar (460-469)", Valor = "460,461,462,463,464,465,466,467,468,469"},
                new { EF = "5920", UB="M133", Grupo = "Pasivo Circulante", Dato = "Préstamos Recibidos y Otras Operaciones Crediticias por Pagar (470-479)", Valor = "470,471,472,473,474,475,476,477,478,479"},
                new { EF = "5920", UB="M134", Grupo = "Pasivo Circulante", Dato = "Gastos Acumulados por Pagar (480-489)", Valor = "480,481,482,483,484,485,486,487,488,489"},
                new { EF = "5920", UB="M135", Grupo = "Pasivo Circulante", Dato = "Provisión para Vacaciones (491;492)", Valor = "491,492"},
                new { EF = "5920", UB="M136", Grupo = "Pasivo Circulante", Dato = "Otras Provosiones Operacionales (493-499)", Valor = "493,494,495,496,497,498,499"},
                new { EF = "5920", UB="M137", Grupo = "Pasivo Circulante", Dato = "Provisión para Pagos de los Subsidios de Seguridad Social a Corto Plazo (500)", Valor = "500"},
                new { EF = "5920", UB="M138", Grupo = "Pasivo Circulante", Dato = "Fondo de Compensación para Desbalances Financieros (501) uso exclusivo de la OSDE", Valor = "501"},
                //Pasivos a Largo Plazo
                new { EF = "5920", UB="M140", Grupo = "Pasivos a Largo Plazo", Dato = "Efectos por Pagar a Largo Plazo (510-514)", Valor = "510,511,512,513,514"},
                new { EF = "5920", UB="M141", Grupo = "Pasivos a Largo Plazo", Dato = "Cuentas por Pagar a Largo Plazo (515-518)", Valor = "515,516,517,518"},
                new { EF = "5920", UB="M152", Grupo = "Pasivos a Largo Plazo", Dato = "Préstamos Recibidos por Pagar a Largo Plazo (520-524)", Valor = "520,521,522,523,524"},
                new { EF = "5920", UB="M153", Grupo = "Pasivos a Largo Plazo", Dato = "Obligaciones a Largo Plazo (525-532)", Valor = "525,526,527,528,529,530,531,532"},
                new { EF = "5920", UB="M154", Grupo = "Pasivos a Largo Plazo", Dato = "Otras Provisiones a Largo Plazo (533-539)", Valor = "533,534,535,536,537,538,539"},
                new { EF = "5920", UB="M155", Grupo = "Pasivos a Largo Plazo", Dato = "Bonos por Pagar (540)-(144)-(363)", Valor = "540,144,363"},
                new { EF = "5920", UB="M156", Grupo = "Pasivos a Largo Plazo", Dato = "Bonos Suscritos (541)", Valor = "541"},					
			    //Pasivos Diferidos
                new { EF = "5920", UB="M158", Grupo = "Pasivos Diferidos", Dato = "Ingresos Diferidos (545-548)", Valor = "545,546,547,548"},
                new { EF = "5920", UB="M159", Grupo = "Pasivos Diferidos", Dato = "Ingresos Diferidos por Donaciones Recibidas (549)", Valor = "549"},
                //Otros Pasivos	
                new { EF = "5920", UB="M161", Grupo = "Otros Pasivos", Dato = "Sobrantes en Investigación (555-564)", Valor = "555,556,557,558,559,560,561,562,563,564"},
                new { EF = "5920", UB="M162", Grupo = "Otros Pasivos", Dato = "Cuentas por Pagar Diversas (565-568)", Valor = "565,566,567,568"},
                new { EF = "5920", UB="M163", Grupo = "Otros Pasivos", Dato = "Cuentas por Pagar- Compra de Moneda (569)", Valor = "569"},
                new { EF = "5920", UB="M164", Grupo = "Otros Pasivos", Dato = "Ingresos de Períodos Futuros (570-574)", Valor = "570,571,572,573,574"},
                new { EF = "5920", UB="M165", Grupo = "Otros Pasivos", Dato = "Obligaciones con el Presupuesto del Estado por Garantía Activada (575)", Valor = "575"},			
	            //PATRIMONIO NETO O CAPITAL CONTABLE
                new { EF = "5920", UB="M169", Grupo = "Patrimonio Neto o Contable", Dato = "Inversión Estatal (600-612) Sector Público", Valor = "600,601,602,603,604,605,606,607,608,609,610,611,612"},
                new { EF = "5920", UB="M170", Grupo = "Patrimonio Neto o Contable", Dato = "Patrimonio y Fondo Común (600) Sector Privado", Valor = "600"},					
	            //Capital Social Suscrito y Pagado 
                new { EF = "5920", UB="M172", Grupo = "Capital Social Suscrito y Pagado", Dato = "Recursos Recibidos (617-618) Sector Publico", Valor = "617,618"},
                new { EF = "5920", UB="M173", Grupo = "Capital Social Suscrito y Pagado", Dato = "Donaciones Recibidas - Nacionales (620)", Valor = "620"},
                new { EF = "5920", UB="M174", Grupo = "Capital Social Suscrito y Pagado", Dato = "Donaciones Recibidas - Exterior (621)", Valor = "621"},
                new { EF = "5920", UB="M175", Grupo = "Capital Social Suscrito y Pagado", Dato = "Utilidad  Retenida (630-634)", Valor = "630,631,632,633,634"},
                new { EF = "5920", UB="M176", Grupo = "Capital Social Suscrito y Pagado", Dato = "Subvención por Pérdida (635-639)", Valor = "635,636,637,638,639"},
                new { EF = "5920", UB="M177", Grupo = "Capital Social Suscrito y Pagado", Dato = "Reservas para Contingencias (645)", Valor = "645"},
                new { EF = "5920", UB="M178", Grupo = "Capital Social Suscrito y Pagado", Dato = "Otras Reservas Patrimoniales (646-654)", Valor = "646,647,648,649,650,651,652,653,654"},
                new { EF = "5920", UB="M179", Grupo = "Capital Social Suscrito y Pagado", Dato = "Fondo de Contravalor para proyectos de Inversión (688)", Valor = "688"},
                new { EF = "5920", UB="M180", Grupo = "Capital Social Suscrito y Pagado", Dato = "Menos: Recursos Entregados (619) Sector Publico	", Valor = "619"},
                new { EF = "5920", UB="M181", Grupo = "Capital Social Suscrito y Pagado", Dato = "Donaciones Entregadas - Nacionales (626)", Valor = "626"},
                new { EF = "5920", UB="M182", Grupo = "Capital Social Suscrito y Pagado", Dato = "Donaciones Entregadas - Exterior (627)", Valor = "627"},
                new { EF = "5920", UB="M183", Grupo = "Capital Social Suscrito y Pagado", Dato = "Pago a Cuenta de Utilidades (690)", Valor = "690"},
                new { EF = "5920", UB="M184", Grupo = "Capital Social Suscrito y Pagado", Dato = "Pago a Cuenta de Dividendos (691)", Valor = "691"},
                new { EF = "5920", UB="M185", Grupo = "Capital Social Suscrito y Pagado", Dato = "Pérdida (640-644)", Valor = "640,641,642,643,644"},
                new { EF = "5920", UB="M186", Grupo = "Capital Social Suscrito y Pagado", Dato = "Mas o Menos: Reevalucion de Activos Fijos Tangibles (613-615)", Valor = "613,614,615"},
                new { EF = "5920", UB="M187", Grupo = "Capital Social Suscrito y Pagado", Dato = "Otras Operaciones de Capital (616-619) Sector Privado", Valor = "616,617,618,619"},
                new { EF = "5920", UB="M188", Grupo = "Capital Social Suscrito y Pagado", Dato = "Revaluación de Inventarios (697)", Valor = "697"},
                new { EF = "5920", UB="M189", Grupo = "Capital Social Suscrito y Pagado", Dato = "Ganancia o Pérdida no Realizada (698)", Valor = "698"},


            };
            return data;
        }

    }

}
