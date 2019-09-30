<template>
  <v-container>
    <v-divider></v-divider>
    <v-row class="my-4 mb-4">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-btn class="mx-2" fab small>
            <v-icon class="mx-2" color="blue darken-4" fab v-on="on" @click="imprimir">mdi-printer</v-icon>
          </v-btn>
        </template>
        <span>Imprimir</span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-btn
            @click="exportTableToExcel"
            class="mx-2 xlsx"
            tableexport-id="51b48fa9-xlsx"
            fab
            small
          >
            <v-icon class="mx-2" color="green" fab v-on="on">mdi-file-excel</v-icon>
          </v-btn>
        </template>
        <span>Exportar a Excel</span>
      </v-tooltip>
    </v-row>

    <v-row class="my-4 mb-4" id="print">
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
            <th class="text-center">plan</th>
            <th class="text-center">real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en relacion a ingresos</th>
            <th class="text-center">% de gastos/total</th>
            <th class="text-center">Hasta junio plan</th>
            <th class="text-center">Hasta junio real</th>
            <th class="text-center">% de cumplimiento</th>
            <th class="text-center">% en ralacion a ingresos</th>
            <th class="text-center">% de gastos/total</th>
          </tr>
        </thead>
        <tbody style="overflow-y:auto;">
          <tr
            v-for="item in ingresos"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Ingresos')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Ingresos')? 'text-right':'colum0']">{{ item.grupo }}</td>
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
            v-for="item in egresos"
            :key="item.grupo"
            v-bind:class="[(item.grupo === 'Egresos')? 'negrita':'']"
          >
            <td v-bind:class="[(item.grupo === 'Egresos')? 'text-right':'colum0']">{{ item.grupo }}</td>
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
            <td
              v-bind:class="[(item.grupo === 'Utilidad')? 'text-right':'colum0']">{{ item.grupo }}</td>
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
    </v-row>
  </v-container>
</template>
<style scoped>
@media print {
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
import api from '@/api';

export default {
  props: ['mes', 'year'],
  data() {
    return {
      ingresos: [],
      egresos: [],
      utilidades: [],
      errors:[],
    };
  },
  created() {
    this.getIngresosFromApi();
    this.getEgresosFromApi();
    this.getUtilidadesFromApi();
  },
  methods: {
    getIngresosFromApi() {
      const url = api.getUrl(
        'finanzas',
        `PlanGI/Ingresos/${this.year}/${this.mes}`,
      );
      this.axios
        .get(url)
        .then((response) => {
          this.ingresos = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(
            'No nos podemos comunicar con el servicio de usuarios, contacte al administrador.',
          );
        });
    },
    getEgresosFromApi() {
      const url = api.getUrl(
        'finanzas',
        `PlanGI/Egresos/${this.year}/${this.mes}`,
      );
      this.axios
        .get(url)
        .then((response) => {
          this.egresos = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(
            'No nos podemos comunicar con el servicio de usuarios, contacte al administrador.',
          );
        });
    },
    getUtilidadesFromApi() {
      const url = api.getUrl(
        'finanzas',
        `PlanGI/Utilidades/${this.year}/${this.mes}`,
      );
      this.axios
        .get(url)
        .then((response) => {
          this.utilidades = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
          vm.$snotify.error(
            'No nos podemos comunicar con el servicio de usuarios, contacte al administrador.',
          );
        });
    },
    imprimir() {
      // Get HTML to print from element
      const prtHtml = document.getElementById("print").innerHTML;

      // Get all stylesheets HTML
      let stylesHtml = "";
      for (const node of [
        ...document.querySelectorAll('link[rel="stylesheet"], style')
      ]) {
        stylesHtml += node.outerHTML;
      }

      // Open the print window
      const WinPrint = window.open(
        "",
        "",
        "left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0"
      );

      WinPrint.document.write(`<!DOCTYPE html>
<html>
  <head>
    ${stylesHtml}
  </head>
  <body>
    ${prtHtml}
  </body>
</html>`);

      WinPrint.document.close();
      WinPrint.focus();
      WinPrint.print();
      WinPrint.close();
    },
    exportTableToExcel(tableID = "table1", filename = "Reportee") {
      var downloadLink;
      var dataType = "application/vnd.ms-excel";
      var tableSelect = document.getElementById("table1");
      var tableHTML = tableSelect.outerHTML.replace(/ /g, "%20");

      // Specify file name
      filename = filename ? filename + ".xls" : "excel_data.xls";


      // Create download link element
      downloadLink = document.createElement("a");

      document.body.appendChild(downloadLink);

      if (navigator.msSaveOrOpenBlob) {
        var blob = new Blob(["\ufeff", tableHTML], {
          type: dataType
        });
        navigator.msSaveOrOpenBlob(blob, filename);
      } else {
        // Create a link to the file
        downloadLink.href = "data:" + dataType + ", " + tableHTML;

        // Setting the file name
        downloadLink.download = filename;
        //triggering the function
        downloadLink.click();
      }
    }
  }
};
</script>
