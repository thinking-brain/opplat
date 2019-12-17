<template>
  <v-simple-table>
    <thead>
      <tr>
        <th>razon</th>
        <th>valor</th>
      </tr>
    </thead>
    <tbody v-for="estado in data" :key="estado.concepto">
      <tr>
        <td class="text-left">{{estado.concepto}}</td>
        <td class="text-left">{{estado.efe}}</td>
      </tr>
      <tr v-for="detalle in estado.detalles" :key="detalle.razon">
        <td>{{detalle.razon}}</td>
        <td>{{detalle.valor}}</td>
      </tr>
    </tbody>
  </v-simple-table>
</template>

<script>
import api from "@/api";

export default {
  props: ["year", "month"],
  data() {
    return {
      year: null,
      month: null
    };
  },
  created(){
    const url = api.getUrl(
        "finanzas",
        `ReporteIngresosGastos/ingresos/${this.year}/${this.mes.id}`
      );
      this.axios
        .get(url)
        .then(response => {
          this.ingresos = response.data;
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

<style>
</style>
