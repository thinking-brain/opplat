<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="line" :options="utilidadesOptions" :series="utilidades_series" />
    </v-card>
  </v-flex>
</template>

<script>
import VueApexCharts from "vue-apexcharts";
import api from "@/api";
export default {
  components: {
    VueApexCharts
  },
  data() {
    return {
      utilidades_series: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
      utilidadesOptions: {
        title: {
          text: "Utilidades",
          align: "center",
          margin: 10,
          offsetX: 0,
          offsetY: 0,
          floating: false,
          style: {
            fontSize: "20px",
            color: "rgba(96, 89, 89, 0.87)"
          }
        },
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
      utilidades_series: [],
      errors: []
    };
  },
  created() {
    var d = new Date();
    var year = d.getFullYear();

    const url = api.getUrl(
      "finanzas",
      `ReporteIngresosGastos/utilidadesTotal/${2019}`
    );
    this.axios
      .get(url)
      .then(response => {
        this.utilidades_series = [
          {
            name: "Real",
            data: response.data.reales
          },
          {
            name: "Plan",
            data: response.data.planes
          }
        ];
      })
      .catch(e => {
        this.errors.push(e);
        vm.$snotify.error(
          "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
        );
      });
  }
};
</script>
