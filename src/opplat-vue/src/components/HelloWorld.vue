<template>
  <v-container fluid >
    <v-layout text-center wrap>
      <v-flex ls12 xs12>
        <charts :options="chartIngresosOptions" />
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  data: () => ({
    chartIngresosOptions:{
      title: {
        text: "Ingresos"
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
      ]}
  }),
};
</script>
