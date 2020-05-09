<template>
  <v-container>
    <h1>{{text}}</h1>
    <v-layout row wrap>
      <Ofertas></Ofertas>
      <Contratos></Contratos>
    </v-layout>
  </v-container>
</template>
<script>
import api from "@/api";
import Ofertas from "@/components/dashboard/Ofertas.vue";
import Contratos from "@/components/dashboard/Contratos.vue";

export default {
  components: {
    Ofertas,
    Contratos
  },
  data: () => ({
    text: "Contratación Dashboard"
  })
};
</script>
