<template>
  <v-container>
    <h1>{{text}}</h1>
    <v-row>
      <v-col cols="12">
        <Ofertas></Ofertas>
      </v-col>
      <v-col cols="12">
        <ContratosProxVen></ContratosProxVen>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
import api from "@/api";
import Ofertas from "@/components/dashboard/contratacion/Ofertas.vue";
import OfertasTotal from "@/components/dashboard/contratacion/OfertasTotal.vue";
import ContratosProxVen from "@/components/dashboard/contratacion/ContratosProxVen.vue";
import ContratosTipo from "@/components/dashboard/contratacion/ContratosTipo.vue";

export default {
  components: {
    Ofertas,
    OfertasTotal,
    ContratosProxVen,
    ContratosTipo
  },
  data: () => ({
    text: "Contratación Dashboard"
  })
};
</script>
