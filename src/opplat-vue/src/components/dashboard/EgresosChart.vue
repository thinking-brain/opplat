<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="line" :options="egresosOptions" :series="egresos_series" />
    </v-card>
  </v-flex>
</template>

<script>
import VueApexCharts from "vue-apexcharts";

export default {
  components: {
    VueApexCharts
  },
  data() {
    return {
      egresosOptions: {
        title: {
          text: "Egresos",
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
      egresos_series: [
        {
          name: "Real",
          data: [30, 40, 45, 50, 49, 60, 70, 91, 87, 45, 58, 69]
        },
        {
          name: "Plan",
          data: [38, 45, 46, 34, 78, 78, 45, 100, 85, 56, 74, 42]
        }
      ]
    };
  }
};
</script>
