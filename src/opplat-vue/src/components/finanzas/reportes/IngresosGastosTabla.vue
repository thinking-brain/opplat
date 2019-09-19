<template>
  <div>
    <v-btn color="blue darken-1" text @click="exportar">Exportar</v-btn>
    <v-btn color="blue darken-1" text @click="imprimir">Imprimir</v-btn>
    <div id="areatoPrint">
      <table id="table1">
        <thead>
          <tr>
            <th colspan="11" class="text-center encabezado1">
              <h2>CNA La Concordia</h2>
            </th>
          </tr>
          <tr>
            <th colspan="11" class="text-center encabezado">
              <h2>Estado del cumplimiento del plan de ingresos y gastos 2019 hasta junio</h2>
            </th>
          </tr>
          <tr>
            <th class="text-center"></th>
            <th class="text-center">junio plan</th>
            <th class="text-center">Junio real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en relacion a ingresos</th>
            <th class="text-center">% de los gastos en función del total</th>
            <th class="text-center">Hasta junio plan</th>
            <th class="text-center">Hasta junio real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en ralacion a ingresos</th>
            <th class="text-center">% de los gastos en función del total</th>
          </tr>
        </thead>
        <tbody style="overflow-y:auto;">
          <tr v-for="item in ingresos" :key="item.grupo" v-bind:class="[(item.grupo === 'Ingresos')? 'negrita':'']">
            <td
              v-bind:class="[(item.grupo === 'Ingresos')? 'text-right':'colum0']"
            >{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>

          <tr v-for="item in egresos" :key="item.grupo" v-bind:class="[(item.grupo === 'Egresos')? 'negrita':'']">
            <td
              v-bind:class="[(item.grupo === 'Egresos')? 'text-right':'colum0']"
            >{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>
          <tr
            v-for="item in utilidades"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Utilidad')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Utilidad')? 'text-right':'colum0']">{{ item.grupo }}</td>
            <td>{{ item.planMes }}</td>
            <td>{{ item.realMes }}</td>
            <td>{{ item.porcCumplimiento }}</td>
            <td>{{item.porcRelacionIngresos}}</td>
            <td>{{item.porcGastosFuncionTotal}}</td>
            <td>{{item.planAcumulado}}</td>
            <td>{{item.realAcumulado}}</td>
            <td>{{item.porcCumpAcumulado}}</td>
            <td>{{item.porcIngresosFuncionTotal}}</td>
            <td>{{item.porcGastosFuncionTotalAcumulado}}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<style scoped>
@media print {
  * {
    display: none;
  }
  #areatoPrint {
    display: block;
  }
  @page {
    size: landscape;
  }
  .tr-th:hover {
    background-color: white !important;
  }
  .encabezado-tabla {
    padding-bottom: 30px;
  }
  table,
  th,
  td {
    border: 1px solid black;
    border-collapse: collapse;
    padding: 5px;
  }
  .encabezado {
    padding-bottom: 20px;
    border-top: none;
  }
  .encabezado1 {
    border-bottom: none;
  }
}
.tr-th:hover {
  background-color: white !important;
}
.encabezado-tabla {
  padding-bottom: 30px;
}
table,
th,
td {
  border: 1px solid black;
  border-collapse: collapse;
  padding: 5px;
  text-align: center;
}
table {
  table-layout: fixed;
  max-width: 100%;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 14px;
}
#areatoPrint {
  overflow-x: auto;
}
td {
  word-wrap: normal;
}
.encabezado {
  padding-bottom: 20px;
  border-top: none;
}
.encabezado1 {
  border-bottom: none;
}
.negrita {
  font-weight: bold;
}
th {
  font-weight: normal;
}
.colum0 {
  text-align: left;
  width: 22%;
}
</style>
<script>
import printJS from "print-js";
export default {
  props: ["mes","year"],
  data() {
    return {
      ingresos: [
        {
          grupo: "Ingresos",
          planMes: 9575000,
          realMes: 1324649.05,
          porcCumplimiento: 18.14,
          porcRelacionIngresos: 100,
          porcGastosFuncionTotal: 0,
          planAcumulado: 16168600,
          realAcumulado: 5106222.65,
          porcCumpAcumulado: 49.72,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Ventas por servicios en moneda nacional",
          planMes: 2202250,
          realMes: 6902.65,
          porcCumplimiento: 0.31,
          porcRelacionIngresos: 0.52,
          porcGastosFuncionTotal: 0,
          planAcumulado: 3718778,
          realAcumulado: 462821.37,
          porcCumpAcumulado: 12.45,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Ventas por servicios en CUC",
          planMes: 7372750,
          realMes: 1314460.56,
          porcCumplimiento: 17.83,
          porcRelacionIngresos: 99.23,
          porcGastosFuncionTotal: 0,
          planAcumulado: 12449822,
          realAcumulado: 4640030.64,
          porcCumpAcumulado: 37.27,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Ingresos financieros",
          planMes: 0,
          realMes: 3285.84,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0.25,
          porcGastosFuncionTotal: 0,
          planAcumulado: 0,
          realAcumulado: 3370.64,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        }
      ],
      egresos: [
        {
          grupo: "Egresos",
          planMes: 3865906.19,
          realMes: 668163.68,
          porcCumplimiento: 83.48,
          porcRelacionIngresos: 100,
          porcGastosFuncionTotal: 100,
          planAcumulado: 6807385.97,
          realAcumulado: 2417808.16,
          porcCumpAcumulado: 191.72,
          porcIngresosFuncionTotal: 13.08,
          porcGastosFuncionTotalAcumulado: 27.64
        },
        {
          grupo: "Impuestos por las ventas",
          planMes: 957500,
          realMes: 132136.32,
          porcCumplimiento: 13.8,
          porcRelacionIngresos: 19.78,
          porcGastosFuncionTotal: 19.78,
          planAcumulado: 1616860,
          realAcumulado: 510285.2,
          porcCumpAcumulado: 31.56,
          porcIngresosFuncionTotal: 2.59,
          porcGastosFuncionTotalAcumulado: 5.47
        },
        {
          grupo: "Costo de ventas",
          planMes: 1053250,
          realMes: 299322.29,
          porcCumplimiento: 28.42,
          porcRelacionIngresos: 44.8,
          porcGastosFuncionTotal: 44.8,
          planAcumulado: 1778546,
          realAcumulado: 988561.99,
          porcCumpAcumulado: 55.58,
          porcIngresosFuncionTotal: 5.86,
          porcGastosFuncionTotalAcumulado: 12.38
        },
        {
          grupo: "Gastos generales de Administracion",
          planMes: 1236137.23,
          realMes: 222237.24,
          porcCumplimiento: 17.98,
          porcRelacionIngresos: 33.26,
          porcGastosFuncionTotal: 33.26,
          planAcumulado: 2295260.46,
          realAcumulado: 849197.71,
          porcCumpAcumulado: 37,
          porcIngresosFuncionTotal: 4.35,
          porcGastosFuncionTotalAcumulado: 9.19
        },
        {
          grupo: "Gastos financieros en MN",
          planMes: 322423.96,
          realMes: 13368.63,
          porcCumplimiento: 4.15,
          porcRelacionIngresos: 2,
          porcGastosFuncionTotal: 2,
          planAcumulado: 609960.35,
          realAcumulado: 64228.86,
          porcCumpAcumulado: 10.53,
          porcIngresosFuncionTotal: 0.26,
          porcGastosFuncionTotalAcumulado: 0.55
        },
        {
          grupo: "Gastos financieros en MLC",
          planMes: 5745,
          realMes: 1099.2,
          porcCumplimiento: 19.13,
          porcRelacionIngresos: 0.16,
          porcGastosFuncionTotal: 0.16,
          planAcumulado: 9701.16,
          realAcumulado: 5534.4,
          porcCumpAcumulado: 57.05,
          porcIngresosFuncionTotal: 0.02,
          porcGastosFuncionTotalAcumulado: 0.05
        },
        {
          grupo: "Gastos por perdida",
          planMes: 3600,
          realMes: 0,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 12000,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Otros impuestos tasas y contribuciones",
          planMes: 287250,
          realMes: 0,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 485058,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        }
      ],
      utilidades: [
        {
          grupo: "Utilidad",
          planMes: 9453235.47,
          realMes: 513550.41,
          porcCumplimiento: 11655.61,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 15182557.19,
          realAcumulado: 334088.5216368,
          porcCumpAcumulado: 102.61,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Pago a cargo de la utilidad",
          planMes: 3734250,
          realMes: 672271.43,
          porcCumplimiento: 18,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 6305754,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Utilidad despues de pagos de anticipos",
          planMes: 1978443.81,
          realMes: -15786.06,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 3067460.03,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Reserva de contingencia del 2% al 10%",
          planMes: 216345.97,
          realMes: 111362.84,
          porcCumplimiento: 51.47,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 325576.93,
          realAcumulado: 334088.5216368,
          porcCumpAcumulado: 102.61,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Utilidad libre despues de la reserva",
          planMes: 1762097.84,
          realMes: -127148.9,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 2741883.1,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Reserva de contingencia 30%",
          planMes: 14417.3,
          realMes: 1670409.2,
          porcCumplimiento: 11586.14,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 21696.46,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        },
        {
          grupo: "Utilidad despues de la reserva del 30%",
          planMes: 1747680.55,
          realMes: -1797558.1,
          porcCumplimiento: 0,
          porcRelacionIngresos: 0,
          porcGastosFuncionTotal: 0,
          planAcumulado: 2720186.67,
          realAcumulado: 0,
          porcCumpAcumulado: 0,
          porcIngresosFuncionTotal: 0,
          porcGastosFuncionTotalAcumulado: 0
        }
      ]
    };
  },
  methods: {
    exportar: function() {
      var TableExport = require("tableexport");
      // dom id, filename, type: json, txt, csv, xml, doc, xsl, image, pdf
      TableExport(document.getElementById("table1"));
    },
    imprimir: function() {
      require("print-js");
      printJS("areatoPrint", "html");
      // var printContents = document.getElementById("areatoPrint").innerHTML;
      // var originalContents = document.body.innerHTML;

      // document.body.innerHTML = printContents;

      // window.print();

      // document.body.innerHTML = originalContents;
    }
  }
};
</script>