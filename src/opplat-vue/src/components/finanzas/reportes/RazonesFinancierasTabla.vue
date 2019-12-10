<template>
  <v-container v-if="visible">
    <v-divider class="d-print-none"></v-divider>
    <v-row class="my-4 mb-4 d-print-none">
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
    <v-row id="print">
      <v-row>
        <table id="table1">
          <thead>
            <tr>
              <th colspan="13" class="text-center encabezado1">
                <img src="img/logo.png" class="float-left" />
                <h2>CNA La Concordia</h2>
                <h2>Razones Finacieras del Año {{year}}</h2>
              </th>
            </tr>
            <tr>
              <th class="text-center">Razón</th>
              <th class="text-center negrita">Enero</th>
              <th class="text-center negrita">Febrero</th>
              <th class="text-center negrita">Marzo</th>
              <th class="text-center negrita">Abril</th>
              <th class="text-center negrita">Mayo</th>
              <th class="text-center negrita">Junio</th>
              <th class="text-center negrita">Julio</th>
              <th class="text-center negrita">Agosto</th>
              <th class="text-center negrita">Septiembre</th>
              <th class="text-center negrita">Octubre</th>
              <th class="text-center negrita">Noviembre</th>
              <th class="text-center negrita">Diciembre</th>
            </tr>
          </thead>
          <tbody v-if="!hasdata">
            <tr>
              <td colspan="13">
                <v-skeleton-loader type="table-row@6"></v-skeleton-loader>
              </td>
            </tr>
          </tbody>
          <tbody v-if="hasdata">
            <tr v-for="item in razones" :key="item.razon">
              <td class="text-right">{{ item.razon}}</td>
              <td class="text-right">{{ item.enero}}</td>
              <td class="text-right">{{ item.febrero}}</td>
              <td class="text-right">{{ item.marzo}}</td>
              <td class="text-right">{{item.abril}}</td>
              <td class="text-right">{{item.mayo}}</td>
              <td class="text-right">{{item.junio}}</td>
              <td class="text-right">{{item.julio}}</td>
              <td class="text-right">{{item.agosto}}</td>
              <td class="text-right">{{item.septiembre}}</td>
              <td class="text-right">{{item.octubre}}</td>
              <td class="text-right">{{item.noviembre}}</td>
              <td class="text-right">{{item.diciembre}}</td>
            </tr>
          </tbody>
        </table>
      </v-row>
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
  table-layout: fixed;
  max-width: 100%;
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
      year: 0,
      razones: null,
      errors: [],
      visible: false,
      hasdata: false
    };
  },
  methods: {
    loadReporte(year) {
      this.year = year;
      this.getRazonesFinancierasFromApi();
    },
    getRazonesFinancierasFromApi() {
      const url = api.getUrl("finanzas", `RazonesFinancieras/${this.year}`);
      this.visible = true;
      this.hasdata = false;
      this.axios
        .get(url)
        .then(response => {
          this.razones = response.data;
          this.hasdata = true;
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
