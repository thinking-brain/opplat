<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card>
      <v-card-title>Razones Financieras</v-card-title>
      <v-simple-table fixed-header>
        <template v-slot:default>
          <thead>
            <tr>
              <th class="text-left">Nombre</th>
              <th class="text-left">Valor</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in data" :key="item.name">
              <td>{{ item.razon }}</td>
              <td>{{ item.valor }}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-card>
  </v-flex>
</template>

<script>
import api from "@/api";
export default {
  data() {
    return {
      data: [{name:'', calories:'0'}]
    };
  },
  created() {
    const year = new Date().getFullYear();
    const mes = new Date().getMonth();
    const url = api.getUrl(
      "finanzas",
      `RazonesFinancieras/MesActual/${year}/${mes}`
    );
    this.axios
      .get(url)
      .then(response => {
        this.data = response.data
      })
      .catch(e => {
        this.errors.push(e);
        vm.$snotify.error(
          "Error al conctarse con la API Finanzas"
        );
      });
  }
};
</script>
