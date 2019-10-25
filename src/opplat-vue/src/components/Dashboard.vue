<template>
  <v-container grid-list-xl fluid>
    <v-layout row wrap>
      <v-flex sm12>
        <h3>Cuadro de Mando</h3>
      </v-flex>
      <v-flex lg12>
        <v-card>
          <v-card-text>
            <charts
              ref="ch"
              :options="chartIngresosOptions"
              :callback="update"
              :updateArgs="[redraw]"
            />
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex sm12>
        <charts :options="chartIngresosOptions" />
      </v-flex>
      <v-flex sm12>
        <charts :options="chartIngresosOptions" />
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  data: () => ({
    chartIngresosOptions: {
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
      reflow: true,
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
          "Dic"
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
      series: [
        {
          name: "Real",
          color: "rgba(11, 230, 20, 0.7)",
          data: [
            29.9,
            71.5,
            106.4,
            129.2,
            144.0,
            176.0,
            135.6,
            148.5,
            216.4,
            194.1,
            95.6,
            54.4
          ]
        },
        {
          name: "Plan",
          color: "#7cb5ec",
          data: [
            216.4,
            194.1,
            95.6,
            54.4,
            29.9,
            71.5,
            106.4,
            129.2,
            144.0,
            176.0,
            135.6,
            148.5
          ]
        }
      ]
    },
    exporting: {
      enabled: true,
      buttons: {
        contextButton: {
          enabled: true,
          text: "Some text"
        }
      }
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
    }
  })
};
</script>
