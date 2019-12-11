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
                //Activo Circulante
                new { EF = "5920", Grupo = "Activo Circulante", Dato = "Efectivo en Caja (101-108)", Valor = "101,102,103,104,105,106,107,108"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Efectivo en Banco y en otras instituciones (109-119)", Valor = "109,110,111,112,113,114,115,116,117,118,119"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Inversiones a Corto Plazo o Temporales (120-129)", Valor = "120,121,122,123,124,125,126,127,128,129"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Efectos por Cobrar a Corto Plazo (130-133)", Valor = "130,131,132,133"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Menos:Efectos por Cobrar Descontados (365-368)", Valor = "365,366,367,368"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Cuenta en Participación (134)", Valor = "134"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Cuentas por Cobrar a Corto Plazo (135-139 y 154)", Valor = "135,136,137,138,139,154"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Menos: Provisión para Cuentas Incobrables (369)", Valor = "369"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Pagos por Cuentas de Terceros (140)", Valor = "140"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Participación de Reaseguradoras por Siniestros Pendientes (141)", Valor = "141"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)", Valor = "142"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Suscriptores de Bonos (143)", Valor = "143"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Pagos Anticipados a Suministradores (146-149)", Valor = "146,147,148,149"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Pagos Anticipados del Proceso Inversionista (150-153)", Valor = "150,151,152,153"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Anticipos a Justificar (161-163)", Valor = "161,162,163"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Adeudos del Presupuesto del Estado (164-166)", Valor = "164,165,166"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Adeudos del Organo u Organismo (167-170)", Valor = "167,168,169,170"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Ingresos acumulados por Cobrar (173-180)", Valor = "173,174,175,176,177,178,179,180"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Dividendos y Participaciones o por Cobrar (181)", Valor = "181"},
                new { EF = "5920", Grupo = "Activo Circulante", Dato ="Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)", Valor = "182"},
                //Total de Inventario
                new { EF = "5920", Grupo = "Total de Inventario", Dato = "Materias Primas y Materiales (183)", Valor = "183"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Combustibles y Lubricantes (184)", Valor = "184"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Partes y Piezas de Respuesto (185)", Valor = "185"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Envases y Embalajes (186)", Valor = "186"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Utiles, Herramientas y Otros (187)", Valor = "187"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Utiles y Herramientas (373)", Valor = "373"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Producción Terminada (188)", Valor = "188"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Mercancias para la Venta (189)", Valor = "189"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Menos: Descuento comercial e Impuesto (370-372)", Valor = "370,371,372"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Medicamentos (190)", Valor = "190"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Base Material de Estudio (191)", Valor = "191"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Base Material de Estudio (366)", Valor = "366"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Vestuario y Lencería (192)", Valor = "192"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Menos: Desgaste de Vestuario y Lenceria (367)", Valor = "367"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Alimentos (193)", Valor = "193"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Inventario de Mercancías de Importación (194)", Valor = "194"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Inventario de Mercancias de Exportación (195)", Valor = "195"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Producciones para Insumo o Autoconsumo (196)", Valor = "196"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Otros Inventarios (205-207)", Valor = "205,206,207"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Inventarios Ociosos (208)", Valor = "208"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Inventarios de lento Movimiento (209)", Valor = "209"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Producción en Proceso (700-724)", Valor = "700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717,718,719,720,721,722,723,724"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Produccion Propia para Insumo o Autoconsumo (725)", Valor = "725"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Reparaciones Capitales con Medios Propios (726)", Valor = "726"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Inversiones con Medios Propios (728)", Valor = "728"},
                new { EF = "5920", Grupo = "Total de Inventario", Dato ="Créditos Documentarios (211)", Valor = "211"},
                //Activos a Largo Plazo
                new { EF = "5920", Grupo = "Activos a Largo Plazo", Dato = "Efectos por Cobrar a Largo Plazo (215-217)", Valor = "215,216,217"},
                new { EF = "5920", Grupo = "Activos a Largo Plazo", Dato ="Cuentas por Cobrar a Largo Plazo (218-220)", Valor = "218,219,220"},
                new { EF = "5920", Grupo = "Activos a Largo Plazo", Dato ="Préstamos Concedidos a Cobrar a Largo Plazo (221-224)", Valor = "221,222,223,224"},
                new { EF = "5920", Grupo = "Activos a Largo Plazo", Dato ="Cuentas en Participación (418-420)", Valor = "418,419,420"},
            };
            return data;
        }



    }
}
