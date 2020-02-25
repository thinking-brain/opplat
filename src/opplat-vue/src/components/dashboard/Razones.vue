<template>
  <v-flex md6 lg6 xs12 pa-2>
    <v-card :elevation="4">
      <v-subheader :inset="inset">
        <h2>Razones Financieras</h2>
      </v-subheader>
      <v-card-text>
        <v-simple-table fixed-header height="500px">
          <thead>
            <tr>
              <th class="text-left">Razon</th>
              <th class="text-left">Valor</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in razones" :key="item.razor">
              <td>{{ item.razon }}</td>
              <td>
                <h3>{{ item.valor }}</h3>
              </td>
            </tr>
          </tbody>
        </v-simple-table>
      </v-card-text>

      <!-- <v-alert
        border="left"
        colored-border
        color="deep-purple accent-4"
        elevation="2"
        v-for="item in razones"
        :key="item.name">
        <v-row align="center">
          <v-col class="grow">
            <h3 class="headline">{{item.name}}</h3>
          </v-col>
          <v-col class="shrink">
            <v-btn>d</v-btn>
          </v-col>
        </v-row>
      </v-alert>-->
    </v-card>
  </v-flex>
</template>

<script>
import api from '@/api';

export default {
  data() {
    return {
      razones: [],
    };
  },
  created() {
    const d = new Date();
    const year = d.getFullYear();
    const month = d.getMonth() + 1;
    const url = api.getUrl(
      'finanzas',
      `RazonesFinancieras/MesActual/${year}/${month}`,
    );
    this.axios
      .get(url)
      .then((response) => {
        this.razones = response.data;
      })
      .catch((e) => {
        this.errors.push(e);
        vm.$snotify.error('Error al conectarse con la API Finanzas');
      });
  },
};
</script>
