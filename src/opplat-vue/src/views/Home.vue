<template>
  <v-container>
    <h2>Cuadro de mando</h2>
    <v-layout row wrap>
      <IngresosChart />
      <EgresosChart />
    </v-layout>
  </v-container>
</template>

<script>
import IngresosChart from "../components/dashboard/IngresosChart";
import EgresosChart from "../components/dashboard/EgresosChart";

export default {
  components: {
    IngresosChart,
    EgresosChart
  }
};
</script>
