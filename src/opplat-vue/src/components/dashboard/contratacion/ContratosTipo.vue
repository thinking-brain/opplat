<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="bar" :options="contratosOptions" :series="contratos" />
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
      contratos: [
        {
          name: "Contratos vencidos",
          data: [2, 3, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0]
        },
        {
          name: "Contratos en proceso  ",
          data: [2, 0, 1, 0, 10, 0, 0, 0, 0, 0, 0, 0]
        }
      ],
      contratosOptions: {
        title: {
          text: "Contratos",
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
          id: "vuechart",
          // mostrar el boton de exportar
          toolbar: {
            show: false
          },
          // definir color de fondo general
          background: ""
        },
        xaxis: {
          // mostrar tooltip para eje x
          tooltip: {
            enabled: false
          },
          // labels para mostrar en eje x
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
          // valor minimo para emezar en eje y
          min: 0,
          // valores redondeados
          forceNiceScale: true
        }
      },
      errors: []
    };
  },
  created() {
    const d = new Date();
    const year = d.getFullYear();
  }
};
</script>
