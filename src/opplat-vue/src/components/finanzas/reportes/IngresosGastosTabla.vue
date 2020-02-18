<template>
  <v-container>
    <v-divider class="d-print-none"></v-divider>
    <v-row v-if="visible" class="my-4 mb-4 d-print-none">
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
    <v-row id="print" v-if="notloading">
      <v-row>
        <table id="table1">
          <thead>
            <tr>
              <th colspan="11" class="text-center encabezado1">
                <img src="img/logo.png" class="float-left" />
                <h2>CNA La Concordia</h2>
                <h2>Estado del cumplimiento del plan de ingresos y gastos {{year}} hasta {{mes.nombre}}</h2>
              </th>
            </tr>
            <tr>
              <th class="text-center"></th>
              <th class="text-center negrita">Plan</th>
              <th class="text-center negrita">Real</th>
              <th class="text-center negrita">% de cumpl</th>
              <th class="text-center negrita">% ingresos</th>
              <th class="text-center negrita">% gastos</th>
              <th class="text-center negrita">Plan acum</th>
              <th class="text-center negrita">Real acum</th>
              <th class="text-center negrita">% de cumpl</th>
              <th class="text-center negrita">% ingresos</th>
              <th class="text-center negrita">% gastos</th>
            </tr>
          </thead>
          <tbody v-if="!hasdata">
            <tr>
              <td colspan="11">
                <v-skeleton-loader type="table-row@6"></v-skeleton-loader>
              </td>
            </tr>
          </tbody>
          <tbody v-if="hasdata">
            <tr
              v-for="item in ingresos"
              :key="item.grupo"
              v-bind:class="[(item.grupo === 'Ingresos')? 'negrita':'']"
            >
              <td
                v-bind:class="[(item.grupo === 'Ingresos')? 'text-right':'colum0']"
              >{{ item.grupo }}</td>
              <td class="text-right">{{ item.planMes | format_two_decimals}}</td>
              <td class="text-right">{{ item.realMes | format_two_decimals}}</td>
              <td class="text-right">{{ item.porcCumplimiento | format_two_decimals}}</td>
              <td class="text-right">{{item.porcRelacionIngresos | format_two_decimals}}</td>
              <td class="deshabilitado"></td>
              <td class="text-right">{{item.planAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.realAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.porcCumpAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.porcIngresosFuncionTotal | format_two_decimals}}</td>
              <td class="deshabilitado"></td>
            </tr>

            <tr
              v-for="item in egresos"
              :key="item.grupo"
              v-bind:class="[(item.grupo === 'Egresos')? 'negrita':'']"
            >
              <td
                v-bind:class="[(item.grupo === 'Egresos')? 'text-right':'colum0']"
              >{{ item.grupo }}</td>
              <td class="text-right">{{ item.planMes | format_two_decimals}}</td>
              <td class="text-right">{{ item.realMes | format_two_decimals }}</td>
              <td class="text-right">{{ item.porcCumplimiento| format_two_decimals}}</td>
              <td class="text-right">{{item.porcRelacionIngresos | format_two_decimals}}</td>
              <td class="text-right">{{item.porcGastosFuncionTotal | format_two_decimals}}</td>
              <td class="text-right">{{item.planAcumulado| format_two_decimals}}</td>
              <td class="text-right">{{item.realAcumulado| format_two_decimals}}</td>
              <td class="text-right">{{item.porcCumpAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.porcIngresosFuncionTotal | format_two_decimals}}</td>
              <td class="text-right">{{item.porcGastosFuncionTotalAcumulado | format_two_decimals}}</td>
            </tr>
            <tr
              v-for="item in utilidades"
              :key="item.grupo"
              v-bind:class="[(item.grupo === 'Utilidad')? 'negrita':'']"
            >
              <td
                v-bind:class="[(item.grupo === 'Utilidad')? 'text-right':'colum0']"
              >{{ item.grupo }}</td>
              <td class="text-right">{{ item.planMes | format_two_decimals }}</td>
              <td class="text-right">{{ item.realMes | format_two_decimals}}</td>
              <td class="text-right">{{ item.porcCumplimiento | format_two_decimals}}</td>
              <td class="text-right">{{item.porcRelacionIngresos | format_two_decimals}}</td>
              <td class="deshabilitado"></td>
              <td class="text-right">{{item.planAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.realAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.porcCumpAcumulado | format_two_decimals}}</td>
              <td class="text-right">{{item.porcIngresosFuncionTotal | format_two_decimals}}</td>
              <td class="deshabilitado"></td>
            </tr>
          </tbody>
        </table>
      </v-row>
      <table class="firmas">
        <tr>
          <th class="text-left">
            <p class="text-center">______________________________</p>
            <p v-if="economico" class="text-center">{{economico.nombre}}</p>
            <p v-if="!economico" class="text-center">Nombre:</p>
            <p v-if="economico" class="text-center">{{economico.cargo}}</p>
            <p v-if="!economico" class="text-center">Economico</p>
          </th>
          <th class="space"></th>
          <th class="text-right">
            <p class="text-center">______________________________</p>
            <p v-if="jefe" class="text-center">{{jefe.nombre}}</p>
            <p v-if="!jefe" class="text-center">Nombre:</p>
            <p v-if="jefe" class="text-center">{{jefe.cargo}}</p>
            <p v-if="!jefe" class="text-center">Jefe</p>
          </th>
        </tr>
      </table>
    </v-row>
  </v-container>
</template>
<style scoped>
@media print {
  #print {
    display: block;
    margin: 10px;
  }
  .v-tooltip__content {
    display: none !important;
  }

  @page {
    size: landscape;
  }
  .tr-th:hover {
    background-color: white !important;
  }
  .space {
    width: 350px !important;
  }
  .encabezado-tabla {
    padding-bottom: 20px;
  }
  table {
    font-family: Arial, Helvetica, sans-serif;
    font-size: 10.5px !important;
  }
  .encabezado {
    padding-bottom: 5px;
    padding-top: 0px;
    border-top: none;
  }
  .encabezado1 {
    border-bottom: none;
  }
}
#print {
  display: block;
  margin: 20px;
}
.firmas,
.firmas > tr > th {
  border: none !important;
  border-collapse: collapse;
  padding: 5px;
}
.space {
  width: 350px !important;
}
.tr-th:hover {
  background-color: white !important;
}
.encabezado-tabla {
  padding-bottom: 20px;
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
  /* table-layout: fixed; */
  max-width: 100%;
  min-width: 100%;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 12px;
}
#areatoPrint {
  overflow-x: auto;
}
td {
  word-wrap: normal;
}
.encabezado {
  padding-bottom: 5px;
  padding-top: 0px;
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
}
.deshabilitado {
  background-color: #e0e0e0;
}
.firmas > tr > th {
  padding-top: 30px;
}
.firmas > tr > th > p {
  margin: 0px;
}
</style>
<script>
import api from "@/api";

export default {
  data() {
    return {
      mes: { id: 0, nombre: "Ninguno" },
      year: 0,
      ingresos: null,
      egresos: null,
      utilidades: null,
      errors: [],
      visible: false
    };
  },
  computed: {
    jefe() {
      if (!this.$store.getters.jefe) {
        this.$store
          .dispatch("cargar")
          .then(() => {})
          .catch(err => {
            console.log(err);
          });
      }
      return this.$store.getters.jefe;
    },
    economico() {
      if (!this.$store.getters.economico) {
        this.$store
          .dispatch("cargar")
          .then(() => {})
          .catch(err => {
            console.log(err);
          });
      }
      return this.$store.getters.economico;
    },
    hasdata() {
      let hd = false;
      if (this.egresos && this.ingresos && this.utilidades) {
        if (
          this.ingresos.length > 0 &&
          this.egresos.length > 0 &&
          this.utilidades.length > 0
        ) {
          hd = true;
        }
      }
      return hd;
    },
    notloading() {
      let hd = false;
      if (this.egresos && this.ingresos && this.utilidades) {
        hd = true;
      }
      return hd;
    }
  },
  methods: {
    loadReporte(mes, year) {
      this.mes = mes;
      this.year = year;
      this.ingresos = [];
      this.utilidades = [];
      this.egresos = [];
      this.getIngresosFromApi();
      this.getEgresosFromApi();
      this.getUtilidadesFromApi();
    },
    getIngresosFromApi() {
      const url = api.getUrl(
        "finanzas",
        `ReporteIngresosGastos/ingresos/${this.year}/${this.mes.id}`
      );
      this.axios
        .get(url)
        .then(response => {
          this.ingresos = response.data;
          this.visible = true;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    getEgresosFromApi() {
      const url = api.getUrl(
        "finanzas",
        `ReporteIngresosGastos/egresos/${this.year}/${this.mes.id}`
      );
      this.axios
        .get(url)
        .then(response => {
          this.egresos = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    getUtilidadesFromApi() {
      const url = api.getUrl(
        "finanzas",
        `ReporteIngresosGastos/utilidad/${this.year}/${this.mes.id}`
      );
      this.axios
        .get(url)
        .then(response => {
          this.utilidades = response.data;
        })
        .catch(e => {
          this.errors.push(e);
          vm.$snotify.error(
            "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
          );
        });
    },
    imprimir() {
      window.print();
    },
    imprimirviejo() {
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
    exportTableToExcel(tableID = "table1", filename = "Reporte ingresos y gastos "+ this.mes.nombre +" "+this.year) {
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
