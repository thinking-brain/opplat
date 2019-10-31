<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="line" :options="ingresosOptions" :series="ingresos_series" />
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
  data: function() {
    return {
      ingresos_reales: [],
      ingresos_planes: [],
      ingresosOptions: {
        title: {
          text: "Ingresos",
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
          //mostrar el boton de exportar
          toolbar: {
            show: false
          },
          //definir color de fondo general
          background: ""
        },
        xaxis: {
          //mostrar tooltip para eje x
          tooltip: {
            enabled: false
          },
          //labels para mostrar en eje x
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
          //valor minimo para emezar en eje y
          min: 0,
          //valores redondeados
          forceNiceScale: true
        }
      },
      ingresos_series: [],
      errors:[]
    };
  },
  created() {
    const url = api.getUrl(
      "finanzas",
      `ReporteIngresosGastos/ingresosTotal/${2019}`
    );
    this.axios
      .get(url)
      .then(response => {
        this.ingresos_series = [
          {
            name: "Real",
            data: response.data.reales
          },
          {
            name: "Plan",
            data: response.data.planes
          },
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
