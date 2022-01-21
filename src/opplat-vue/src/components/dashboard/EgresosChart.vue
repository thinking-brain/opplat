<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <VueApexCharts type="line" :options="egresosOptions" :series="egresos_series" />
    </v-card>
  </v-flex>
</template>

<script>
import VueApexCharts from 'vue-apexcharts';
import api from '@/api';

export default {
  components: {
    VueApexCharts,
  },
  data() {
    return {
      egresos_series: [
        {
          name: 'Real',
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        },
        {
          name: 'Plan',
          data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        },
      ],
      egresosOptions: {
        title: {
          text: 'Egresos',
          align: 'center',
          margin: 10,
          offsetX: 0,
          offsetY: 0,
          floating: false,
          style: {
            fontSize: '20px',
            color: 'rgba(96, 89, 89, 0.87)',
          },
        },
        chart: {
          id: 'vuechart',
        },
        xaxis: {
          tooltip: {
            enabled: false,
          },
          categories: [
            'Enero',
            'Febrero',
            'Marzo',
            'Abril',
            'Mayo',
            'Junio',
            'Julio',
            'Agosto',
            'Septiembre',
            'Octubre',
            'Noviembre',
            'Diciembre',
          ],
        },
        yaxis: {
          min: 0,
          forceNiceScale: true,
        },
      },
      egresos_series: [],
      errors: [],
    };
  },
  created() {
    const d = new Date();
    const year = d.getFullYear();

    const url = api.getUrl(
      'finanzas',
      `ReporteIngresosGastos/egresosTotal/${year}`,
    );
    this.axios
      .get(url)
      .then((response) => {
        this.egresos_series = [
          {
            name: 'Real',
            data: response.data.reales,
          },
          {
            name: 'Plan',
            data: response.data.planes,
          },
        ];
      })
      .catch((e) => {
        this.errors.push(e);
        vm.$snotify.error(
          'No nos podemos comunicar con el servicio de usuarios, contacte al administrador.',
        );
      });
  },
};
</script>
