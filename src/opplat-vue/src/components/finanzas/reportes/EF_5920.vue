<template>
  <v-simple-table>
    <thead>
      <tr>
        <th>razon</th>
        <th>valor</th>
      </tr>
    </thead>
    <tbody v-for="estado in data" :key="estado.concepto">
      <tr>
        <td class="text-left">{{estado.concepto}}</td>
        <td class="text-left">{{estado.efe}}</td>
      </tr>
      <tr v-for="detalle in estado.detalles" :key="detalle.razon">
        <td>{{detalle.razon}}</td>
        <td>{{detalle.valor}}</td>
      </tr>
    </tbody>
  </v-simple-table>
</template>

<script>
import api from '@/api';

export default {
  props: ['year', 'mes'],
  data() {
    return {
      data: [
        {
          concepto: 'Activo Circulante',
          efe: '5920',
          detalles: [
            {
              razon: 'Efectivo en Caja (101-108)',
              valor: 72799.42,
            },
            {
              razon: 'Efectivo en Banco y en otras instituciones (109-119)',
              valor: -125637.42,
            },
            {
              razon: 'Inversiones a Corto Plazo o Temporales (120-129)',
              valor: 0,
            },
            {
              razon: 'Efectos por Cobrar a Corto Plazo (130-133)',
              valor: 0,
            },
            {
              razon: 'Menos:Efectos por Cobrar Descontados (365-368)',
              valor: 0,
            },
            {
              razon: 'Cuenta en Participación (134)',
              valor: 0,
            },
            {
              razon: 'Cuentas por Cobrar a Corto Plazo (135-139 y 154)',
              valor: -492414.05,
            },
            {
              razon: 'Menos: Provisión para Cuentas Incobrables (369)',
              valor: 0,
            },
            {
              razon: 'Pagos por Cuentas de Terceros (140)',
              valor: 0,
            },
            {
              razon:
                'Participación de Reaseguradoras por Siniestros Pendientes (141)',
              valor: 0,
            },
            {
              razon:
                'Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo (142)',
              valor: 0,
            },
            {
              razon: 'Suscriptores de Bonos (143)',
              valor: 0,
            },
            {
              razon: 'Pagos Anticipados a Suministradores (146-149)',
              valor: 27600,
            },
            {
              razon: 'Pagos Anticipados del Proceso Inversionista (150-153)',
              valor: 0,
            },
            {
              razon: 'Anticipos a Justificar (161-163)',
              valor: 0,
            },
            {
              razon: 'Adeudos del Presupuesto del EstadoOrden (164-166)',
              valor: -1386610.07,
            },
            {
              razon: 'Adeudos del Organo u Organismo (167-170)',
              valor: 0,
            },
            {
              razon: 'Ingresos acumulados por Cobrar (173-180)',
              valor: 0,
            },
            {
              razon: 'Dividendos y Participaciones o por Cobrar (181)',
              valor: 0,
            },
            {
              razon:
                'Ingresos Acumulados por Cobrar - Reaseguros Aceptados (182)',
              valor: 0,
            },
          ],
        },
        {
          concepto: 'Total de Inventario',
          efe: '5920',
          detalles: [
            {
              razon: 'Materias Primas y Materiales (183)',
              valor: -39986.22,
            },
            {
              razon: 'Combustibles y Lubricantes (184)',
              valor: 0,
            },
            {
              razon: 'Partes y Piezas de Respuesto (185)',
              valor: 0,
            },
            {
              razon: 'Envases y Embalajes (186)',
              valor: 0,
            },
            {
              razon: 'Utiles, Herramientas y Otros (187)',
              valor: 240,
            },
            {
              razon: 'Menos: Desgaste de Utiles y Herramientas (373)',
              valor: 0,
            },
            {
              razon: 'Producción Terminada (188)',
              valor: 0,
            },
            {
              razon: 'Mercancias para la Venta (189)',
              valor: 0,
            },
            {
              razon: 'Menos: Descuento comercial e Impuesto (370-372)',
              valor: 0,
            },
            {
              razon: 'Medicamentos (190)',
              valor: 0,
            },
            {
              razon: 'Base Material de Estudio (191)',
              valor: 0,
            },
            {
              razon: 'Menos: Desgaste de Base Material de Estudio (366)',
              valor: 0,
            },
            {
              razon: 'Vestuario y Lencería (192)',
              valor: 0,
            },
            {
              razon: 'Menos: Desgaste de Vestuario y Lenceria (367)',
              valor: 0,
            },
            {
              razon: 'Alimentos (193)',
              valor: 0,
            },
            {
              razon: 'Inventario de Mercancías de Importación (194)',
              valor: 0,
            },
            {
              razon: 'Inventario de Mercancias de Exportación (195)',
              valor: 0,
            },
            {
              razon: 'Producciones para Insumo o Autoconsumo (196)',
              valor: 0,
            },
            {
              razon: 'Otros Inventarios (205-207)',
              valor: 0,
            },
            {
              razon: 'Inventarios Ociosos (208)',
              valor: 0,
            },
            {
              razon: 'Inventarios de lento Movimiento (209)',
              valor: 0,
            },
            {
              razon: 'Producción en Proceso (700-724)',
              valor: 8591.73,
            },
            {
              razon: 'Produccion Propia para Insumo o Autoconsumo (725)',
              valor: 0,
            },
            {
              razon: 'Reparaciones Capitales con Medios Propios (726)',
              valor: 0,
            },
            {
              razon: 'Inversiones con Medios Propios (728)',
              valor: 0,
            },
            {
              razon: 'Créditos Documentarios (211)',
              valor: 0,
            },
          ],
        },
        {
          concepto: 'Activos a Largo Plazo',
          efe: '5920',
          detalles: [
            {
              razon: 'Efectos por Cobrar a Largo Plazo (215-217)',
              valor: 0,
            },
            {
              razon: 'Cuentas por Cobrar a Largo Plazo (218-220)',
              valor: 0,
            },
            {
              razon: 'Préstamos Concedidos a Cobrar a Largo Plazo (221-224)',
              valor: 0,
            },
            {
              razon: 'Cuentas en Participación (418-420)',
              valor: 0,
            },
          ],
        },
      ],
    };
  },
  created() {
    // const url = api.getUrl(
    //     "finanzas",
    //     `ReporteIngresosGastos/ingresos/${this.year}/${this.mes.id}`
    //   );
    //   this.axios
    //     .get(url)
    //     .then(response => {
    //       this.ingresos = response.data;
    //     })
    //     .catch(e => {
    //       this.errors.push(e);
    //       vm.$snotify.error(
    //         "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
    //       );
    //     });
    console.log(this.year, this.mes);
  },
};
</script>

<style>
</style>
