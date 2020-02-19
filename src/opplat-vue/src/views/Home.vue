<template>
  <v-container>
    <h2>Cuadro de mando</h2>
    <v-layout row wrap>
      <IngresosChart />
      <EgresosChart />
      <Utilidades />
      <Razones />
    </v-layout>
  </v-container>
</template>

<script>
import IngresosChart from '../components/dashboard/IngresosChart';
import EgresosChart from '../components/dashboard/EgresosChart';
import Razones from '../components/dashboard/Razones';
import Utilidades from '../components/dashboard/Utilidades';

export default {
  components: {
    IngresosChart,
    EgresosChart,
    Utilidades,
    Razones,
  },
};
</script>
