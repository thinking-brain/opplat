using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.Data
{
    public class DatosVariablesEstadoFinanciero
    {
        public static List<dynamic> Datos()
        {
            var data = new List<dynamic> {
                //Activo Circulante
                new { Dato = "Cuenta en Participación", Valor = "134"},
                new { Dato = "Cuentas por Cobrar a Corto Plazo", Valor = "135,136,137,138,139,154"},
                new { Dato = "Efectivo en Caja", Valor = "101,102,103,104,105,106,107,108"},
                new { Dato = "Efectivo en Banco y en otras instituciones", Valor = "109,110,111,112,113,114,115,116,117,118,119"},
                new { Dato = "Inversiones a Corto Plazo o Temporales", Valor = "120,121,122,123,124,125,126,127,128,129"},
                new { Dato = "Efectos por Cobrar a Corto Plazo", Valor = "130,131,132,133"},
                new { Dato = "Pagos por Cuentas de Terceros", Valor = "140"},
                new { Dato = "Créditos Documentarios", Valor = "211"},
                new { Dato = "Participación de Reaseguradoras por Siniestros Pendientes", Valor = "141"},
                new { Dato = "Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo", Valor = "142"},
                new { Dato = "Suscriptores de Bonos", Valor = "143"},
                new { Dato = "Pagos Anticipados a Suministradores", Valor = "146,147,148,149"},
                new { Dato = "Pagos Anticipados del Proceso + Inversionista", Valor = "150,151,152,153"},
                new { Dato = "Anticipos a Justificar", Valor = "161,162,163"},
                new { Dato = "Adeudos del Presupuesto del Estado", Valor = "164,165,166"},
                new { Dato = "Adeudos del Organo u Organismo", Valor = "167,168,169,170"},
                new { Dato = "Ingresos acumulados por Cobrar", Valor = "173,174,175,176,177,178,179,180"},
                new { Dato = "Dividendos y Participaciones o por Cobrar", Valor = "181"},
                new { Dato = "Ingresos Acumulados por Cobrar - Reaseguros Aceptados", Valor = "182"},
                new { Dato = "Efectos por Cobrar Descontados", Valor = "365,366,367,368"},
                new { Dato = "Provisión para Cuentas Incobrables", Valor = "369"},
                // Total de Inventario
                new { Dato = "Materias Primas y Materiales", Valor = "183"},
                new { Dato = "Combustibles y Lubricantes", Valor = "184"},
                new { Dato = "Partes y Piezas de Respuesto", Valor = "185"},
                new { Dato = "Envases y Embalajes", Valor = "186"},
                new { Dato = "Utiles, Herramientas y Otros", Valor = "187"},
                new { Dato = "Producción Terminada", Valor = "188"},
                new { Dato = "Mercancias para la Venta", Valor = "189"},
                new { Dato = "Medicamentos", Valor = "190"},
                new { Dato = "Base Material de Estudio", Valor = "191"},
                new { Dato = "Vestuario y Lencería", Valor = "192"},
                new { Dato = "Alimentos", Valor = "193"},
                new { Dato = "Inventario de Mercancías de Importación", Valor = "194"},
                new { Dato = "Inventario de Mercancias de Exportación", Valor = "195"},
                new { Dato = "Producciones para Insumo o Autoconsumo", Valor = "196"},
                new { Dato = "Otros Inventarios", Valor = "205,206,207"},
                new { Dato = "Inventarios Ociosos", Valor = "208"},
                new { Dato = "Inventarios de lento Movimiento", Valor = "209"},
                new { Dato = "Producción en Proceso", Valor = "700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717,718,719,720,721,722,723,724"},
                new { Dato = "Produccion Propia para Insumo o Autoconsumo", Valor = "725"},
                new { Dato = "Reparaciones Capitales con Medios Propios", Valor = "726"},
                new { Dato = "Inversiones con Medios Propios", Valor = "728"},
                new { Dato = "Desgaste de Utiles y Herramientas", Valor = "373"},
                new { Dato = "Desgaste de Base Material de Estudio", Valor = "366"},
                new { Dato = "Desgaste de Vestuario y Lenceria", Valor = "367"},
               
               //Activos a Largo Plazo
               
                new { Dato = "Efectos por Cobrar a Largo Plazo", Valor = "215,217"},
                new { Dato = "Cuentas por Cobrar a Largo Plazo", Valor = "218,220"},
                new { Dato = "Préstamos Concedidos a Cobrar a Largo Plazo", Valor = "221,224"},
                new { Dato = "Inversiones a Largo Plazo o Permanentes", Valor = "225,234"},
               

               //Activos Fijos

                new { Dato = "Medios y Equipos para Alquilar", Valor = "253"},
                new { Dato = "Fondos Bibliotecarios", Valor = "252"},
                new { Dato = "Activos Fijos Tangibles", Valor = "240,241,242,243,244,245,246,247,248,249,250,251"},
                new { Dato = "Monumentos y Obras de Arte", Valor = "254"},
                new { Dato = "Activos Fijos Intangibles", Valor = "255,256,257,258,259,260,261,262,263,264"},
                new { Dato = "Inversiones en Proceso", Valor = "265,266,267,268,269,270,271,272,273,274,275,276,277,278,279"},
                new { Dato = "Equipos por Instalar y Materiales para el Proceso Inversionista", Valor = "280,281,282,283,284,285,286,287,288,289"},
                new { Dato = "Depreciación de Activos Fijos Tangibles", Valor = "375,376,377,378,379,380,381,382,383,384,385,386,387,388"},
                new { Dato = "Desgaste de Medios y Equipos para Alquilar", Valor = "389"},
                new { Dato = "Amortización de Activos Fijos Intangibles", Valor = "390,391,392,393,394,395,396,397,398,399"},
                
                //Activos Diferidos
                new { Dato = "Gastos de Produccion y Servicios Diferidos", Valor = "300,301,302,303,304,305"},
                new { Dato = "Gastos Financieros Diferidos", Valor = "306,307"},
                new { Dato = "Gastos Diferidos del Proceso Inversionista", Valor = "310,311"},

                //Otros Activos
                new { Dato = "Pérdidas en Investigación", Valor = "330,331"},
                new { Dato = "Faltante de Bienes en Investigación", Valor = "332,333"},
                new { Dato = "Cuentas por Cobrar Diversas - Operaciones Corrientes", Valor = "334,335,336,337,338,339,340,341"},
                new { Dato = "Cuentas por Cobrar - Compra de Monedas", Valor = "342"},
                new { Dato = "Cuentas por Cobrar Diversa del Proceso Inversionista", Valor = "343,344,345"},
                new { Dato = "Efectos por Cobrar en Litigio", Valor = "346"},
                new { Dato = "Cuentas por Cobrar en Litigio", Valor = "347"},
                new { Dato = "Efectos por Cobrar Protestados", Valor = "348"},
                new { Dato = "Cuentas por Cobrar en Proceso Judicial", Valor = "349"},
                new { Dato = "Depósitos y Fianzas", Valor = "354,355"},
                new { Dato = "Fondo de Amortización de Bonos - Efectivo en Valores", Valor = "364"},
                new { Dato = "Otras Provisiones Reguladoras de Activos", Valor = "374"},

                //Pasivo Circulante	

                new { Dato = "Sobregiro Bancario", Valor = "400"},
                new { Dato = "Efectos por Pagar a Corto Plazo", Valor = "401,402,403,404"},
                new { Dato = "Cuentas por Pagar a Corto Plazo", Valor = "405,406,407,408,409,410,411,412,413,414,415"},
                new { Dato = "Cobros por Cuenta de Terceros", Valor = "416"},
                new { Dato = "Dividendos y Participaciones por Pagar", Valor = "417"},
                new { Dato = "Cuentas en Participación", Valor = "418,419,420"},
                new { Dato = "Cuentas por Pagar-Activos Fijos Tangibles", Valor = "421,422,423,424"},
                new { Dato = "Cuentas por Pagar del Proceso Inversionista", Valor = "425,426,427,428,429"},
                new { Dato = "Cobros Anticipados", Valor = "430,431,432,433,434"},
                new { Dato = "Depósitos Recibidos", Valor = "435,436,437,438,439"},
                new { Dato = "Obligaciones con el Presupuesto del Estado", Valor = "440,441,442,443,444,445,446,447,448,449"},
                new { Dato = "Obligaciones con el Organo u Organismo", Valor = "450,451,452,453"},
                new { Dato = "Nóminas por Pagar", Valor = "455,456,457,458,459"},
                new { Dato = "Retenciones por Pagar", Valor = "460,461,462,463,464,465,466,467,468,469"},
                new { Dato = "Préstamos Recibidos y Otras Operaciones Crediticias por Pagar", Valor = "470,471,472,473,474,475,476,477,478,479"},
                new { Dato = "Gastos Acumulados por Pagar", Valor = "480,481,482,483,484,485,486,487,488,489"},
                new { Dato = "Provisión para Vacaciones", Valor = "492"},
                new { Dato = "Otras Provosiones Operacionales", Valor = "493,494,495,496,497,498,499"},
                new { Dato = "Provisión para Pagos de los Subsidios de Seguridad Social a Corto Plazo", Valor = "500"},
                new { Dato = "Fondo de Compensación para Desbalances Financieros", Valor = "501"},

                //Pasivos a largo plazo
                new { Dato = "Efectos por Pagar a Largo Plazo", Valor = "510,511,512,513,514"},
                new { Dato = "Cuentas por Pagar a Largo Plazo", Valor = "515,516,517,518,519"},
                new { Dato = "Préstamos Recibidos por Pagar a Largo Plazo", Valor = "520,521,522,523,524"},
                new { Dato = "Obligaciones a Largo Plazo", Valor = "525,526,527,528,529,530,531,532"},
                new { Dato = "Otras Provisiones a Largo Plazo", Valor = "533,534,535,536,537,538,539"},
                //OJO CON ESTA ES UNA RESTA DE LAS 3 CUENTAS
                new { Dato = "Bonos por Pagar", Valor = "540,144,363"},
                new { Dato = "Bonos Suscritos", Valor = "541"},

                //Pasivos Diferidos
                new { Dato = "Ingresos Diferidos", Valor = "545,546,547,548"},
                new { Dato = "Ingresos Diferidos por Donaciones Recibidas ", Valor = "549"},
                //Otros Pasivos
                new { Dato = "Sobrantes en Investigación", Valor = "555,556,557,558,559,560,561,562,563,564"},
                new { Dato = "Cuentas por Pagar Diversas", Valor = "565,566,567,568"},
                new { Dato = "Cuentas por Pagar - Compra de Moneda ", Valor = "569"},
                new { Dato = "Ingresos de Períodos Futuros", Valor = "570,571,572,573,574"},
              
             
          
                //Total de Patrimonio Neto
                new { Dato = "Inversión Estatal", Valor = "600,601,602,603,604,605,606,607,608,609,610,611,612"},
                new { Dato = "Sector Público + Patrimonio", Valor = "600"},
                new { Dato = "Sector Privado + Capital Social Suscrito y Pagado + Recursos Recibidos", Valor = "617,618"},
                new { Dato = "Sector Publico + Donaciones Recibidas - Nacionales", Valor = "620"},
                new { Dato = "Donaciones Recibidas - Exterior", Valor = "621"},
                new { Dato = "Utilidad Retenida", Valor = "630,631,632,633,634"},
                new { Dato = "Subvención por Pérdida Sector Publico", Valor = "635,636,637,638,639"},
                new { Dato = "Reservas para Contingencias", Valor = "645"},
                new { Dato = "Otras Reservas Patrimoniales", Valor = "646,647,648,649,650,651,652,653,654"},
                new { Dato = "Recursos Entregados", Valor = "619"},
                new { Dato = "Sector Publico + Donaciones Entregadas - Nacionales", Valor = "600"},
                new { Dato = "Donaciones Entregadas - Exterior", Valor = "627"},
                new { Dato = "Pago a Cuenta de Utilidades", Valor = "690"},
                new { Dato = "Pago a Cuenta de Dividendos", Valor = "691"},
                new { Dato = "Pérdida", Valor = "640,641,642,643,644"},
                new { Dato = "Reevalucion de Activos Fijos Tangibles", Valor = "613,614,615"},
                new { Dato = "Otras Operaciones de Capital", Valor = "616,617,618,619"},
                new { Dato = "Sector Privado Revaluación de Inventarios", Valor = "697"},
                new { Dato = "Ganancia o Pérdida no Realizada", Valor = "698"},



            };

            return data;
        }
    }
}
