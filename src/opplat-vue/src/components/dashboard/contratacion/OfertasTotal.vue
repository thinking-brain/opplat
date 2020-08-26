<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="bar" :options="ofertasOptions" :series="ofertas" />
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
      ofertas: [
        {
          name: "Ofertas vencidas",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        },
        {
          name: "Ofertas en proceso",
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        }
      ],
      ofertasOptions: {
        title: {
          text: "Total de Ofertas",
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
            "6 Meses",
            "3 Meses",
            "Mes Actual",
           
          ]
        },
        yaxis: {
          // valor minimo para emezar en eje y
          min: 0,
          // valores redondeados
          forceNiceScale: true
        }
      },
      ofertas: [],
      errors: []
    };
  },
  created() {
    const d = new Date();
    const year = d.getFullYear();
    this.getOfertasFromApi();
  },
  methods: {
    getOfertasFromApi() {
      const url = api.getUrl("contratacion", "contratos/Dashboard");
      this.axios.get(url).then(
        response => {
          this.ofertas = [
            {
              name: "Ofertas Vencidas",
              data: response.data.ofertasVencidas
            },
            {
              name: "Ofertas en Proceso",
              data: response.data.ofertasProceso
            }
          ];
        },
        error => {
          console.log(error);
        }
      );
    }
  }
};
</script>
