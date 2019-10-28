<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Cuadro de Mando</h3>
      </v-flex>
      <v-flex md6 lg6>
        <v-card flex>
          <charts :options="chartIngresosOptions" />
        </v-card>
      </v-flex>
      <!-- <v-flex sm12>
        <charts :options="chartIngresosOptions" />
      </v-flex>
      <v-flex sm12>
        <charts :options="chartIngresosOptions" />
      </v-flex>-->
    </v-layout>
  </v-container>
</template>

<script>
import api from "@/api";

export default {
  data: () => ({
    ingresos: {
      planes: [],
      reales: [],
      planAcum: 0,
      realAcum: 0
    },
    chartIngresosOptions: null
  }),
  created() {
    const url = api.getUrl(
      "finanzas",
      `ReporteIngresosGastos/ingresosTotal/${2019}`
    );
    this.axios
      .get(url)
      .then(response => {
        this.ingresos.reales = response.data.reales;
        this.ingresos.planes = response.data.planes;
        this.ingresos.reales.push(response.data.real);
        this.ingresos.planes.push(response.data.plan);

        this.chartIngresosOptions.series = [
          {
            name: "Real",
            color: "rgba(11, 230, 20, 0.7)",
            data: this.ingresos.reales
          },
          {
            name: "Plan",
            color: "#7cb5ec",
            data: this.ingresos.planes
          }
        ];
      })
      .catch(e => {
        this.errors.push(e);
        vm.$snotify.error(
          "No nos podemos comunicar con el servicio de usuarios, contacte al administrador."
        );
      });

    this.chartIngresosOptions = {
      title: {
        text: "Ingresos"
      },
      responsive: {
        rules: [
          {
            condition: {
              maxWidth: 500
            },
            chartOptions: {
              legend: {
                layout: "horizontal",
                align: "center",
                verticalAlign: "bottom"
              }
            }
          }
        ]
      },
      chart: {
        reflow: true
      },
      xAxis: {
        categories: [
          "Ene",
          "Feb",
          "Mar",
          "Abr",
          "May",
          "Jun",
          "Jul",
          "Ago",
          "Sep",
          "Oct",
          "Nov",
          "Dic",
          "Acumulado"
        ]
      },
      legend: {
        layout: "vertical",
        align: "right",
        verticalAlign: "middle"
      },
      yAxis: {
        title: {
          text: "Valores"
        }
      },
      credits: {
        enabled: false
      },
      series: [],
      responsive: {
        rules: [
          {
            condition: {
              maxWidth: 500
            },
            chartOptions: {
              legend: {
                layout: "horizontal",
                align: "center",
                verticalAlign: "bottom"
              }
            }
          }
        ]
      },
      exporting: {
        enabled: true,
        buttons: {
          contextButton: {
            enabled: true
          }
        }
      }
    };
  }
};
</script>
