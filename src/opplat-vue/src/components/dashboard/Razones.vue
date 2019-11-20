<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <v-card-title>Razones Financieras</v-card-title>
      <v-simple-table fixed-header height="300px">
        <template v-slot:default>
          <thead>
            <tr>
              <th class="text-left">Name</th>
              <th class="text-left">Calories</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in data" :key="item.name">
              <td>{{ item.name }}</td>
              <td>{{ item.calories }}</td>
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
      data: [{name:'Vicente', calories:'345'}]
    };
  },
  created() {
    const url = api.getUrl(
      "finanzas",
      `RazonesFinancieras/MesActual/${2019}/${2}`
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
