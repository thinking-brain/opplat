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
              <td class="text-left">{{ item.razon }}</td>
              <td class="text-right">{{ item.enero| format_two_decimals}}</td>
              <td class="text-right">{{ item.febrero| format_two_decimals}}</td>
              <td class="text-right">{{ item.marzo| format_two_decimals}}</td>
              <td class="text-right">{{item.abril| format_two_decimals}}</td>
              <td class="text-right">{{item.mayo| format_two_decimals}}</td>
              <td class="text-right">{{item.junio| format_two_decimals}}</td>
              <td class="text-right">{{item.julio| format_two_decimals}}</td>
              <td class="text-right">{{item.agosto| format_two_decimals}}</td>
              <td class="text-right">{{item.septiembre| format_two_decimals}}</td>
              <td class="text-right">{{item.octubre| format_two_decimals}}</td>
              <td class="text-right">{{item.noviembre| format_two_decimals}}</td>
              <td class="text-right">{{item.diciembre| format_two_decimals}}</td>
            </tr>
          </tbody>
        </table>
      </v-row>
    </v-row>
    <v-row v-if="hasdata">
      <v-flex xs12 pa-2>
        <v-card :elevation="4">
          <v-card-title>
            <p class="razones_subtitle">Comportamiento de las razones en el año</p>
            <v-spacer></v-spacer>
            <v-col class="d-flex" cols="12" sm="4">
              <v-select
                v-model="tipo_razon_selected"
                :items="tipos_de_razones"
                label="Seleccione tipo de razón"
                solo
                item-text="name"
                item-value="id"
              ></v-select>
            </v-col>
          </v-card-title>

          <VueApexCharts type="line" :options="razonesOptions" :series="razones_series" />
        </v-card>
      </v-flex>
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
.razones_subtitle {
  font-size: 20px;
  color: rgba(96, 89, 89, 0.87);
  font-family: Arial, Helvetica, sans-serif;
}
</style>
<script>
import VueApexCharts from "vue-apexcharts";
import api from "@/api";

export default {
  components: {
    VueApexCharts
  },
  data() {
    return {
      year: 0,
      razones: null,
      errors: [],
      visible: false,
      hasdata: false,
      razones_series: [
        {
          name: "Real",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        },
        {
          name: "Plan",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        }
      ],
      tipo_razon_selected: null,
      tipos_de_razones: [
        { id: 1, name: "Razones de Liquidéz" },
        { id: 2, name: "Razones de Endeudamiento" },
        { id: 3, name: "Razones de Rentabilidad" }
      ],
      razonesOptions: {
        // title: {
        //   text: "Comportamiento de las razones en el año",
        //   align: "center",
        //   margin: 10,
        //   offsetX: 0,
        //   offsetY: 0,
        //   floating: false,
        //   style: {
        //     fontSize: "20px",
        //     color: "rgba(96, 89, 89, 0.87)"
        //   }
        // },
        chart: {
          id: "vuechart"
        },
        xaxis: {
          tooltip: {
            enabled: false
          },
          categories: [
            "Enero",
            "Febrero",
            "Marzo",
            "Abril",
            "Mayo",
            "Junio",
            "Julio",
            "Agosto",
            "Septiembre",
            "Octubre",
            "Noviembre",
            "Diciembre"
          ]
        },
        yaxis: {
          min: 0,
          forceNiceScale: true
        }
      },
      razones_series: [],
      errors: []
    };
  },
  watch: {
    tipo_razon_selected: function() {
      var r = this.razones.filter(r => r.tipo === this.tipo_razon_selected);
      this.razones_series = [];
      r.forEach(element => {
        this.razones_series.push({
          name: element.razon,
          data: [
            element.enero,
            element.febrero,
            element.marzo,
            element.abril,
            element.mayo,
            element.junio,
            element.julio,
            element.agosto,
            element.septiembre,
            element.octubre,
            element.noviembre,
            element.diciembre
          ]
        });
      });
      console.log(r);
    }
  },
  methods: {
    razonesFilter() {
      var r = this.razones_series;
      this.razones_series = r.filter(
        razon => razon.tipo == this.tipo_razon_selected
      );
      console.log(this.razones_series);
    },
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
          this.tipo_razon_selected = 1;
          this.hasdata = true;
        })
        .catch(e => {
          this.errors.push(e);
          console.log(e);
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
      let downloadLink;
      const dataType = "application/vnd.ms-excel";
      const tableSelect = document.getElementById("table1");
      const tableHTML = tableSelect.outerHTML.replace(/ /g, "%20");

      // Specify file name
      filename = filename ? `${filename}.xls` : "excel_data.xls";

      // Create download link element
      downloadLink = document.createElement("a");

      document.body.appendChild(downloadLink);

      if (navigator.msSaveOrOpenBlob) {
        const blob = new Blob(["\ufeff", tableHTML], {
          type: dataType
        });
        navigator.msSaveOrOpenBlob(blob, filename);
      } else {
        // Create a link to the file
        downloadLink.href = `data:${dataType}, ${tableHTML}`;

        // Setting the file name
        downloadLink.download = filename;
        // triggering the function
        downloadLink.click();
      }
    }
  }
};
</script>
